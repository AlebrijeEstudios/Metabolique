using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Feeding;
using AppVidaSana.Models.Dtos.Feeding_Dtos;
using AppVidaSana.Models.Feeding;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services
{
    public class FeedingService : IFeeding
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;
        private readonly ValidationValuesDB _validationValues;
  
        public FeedingService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
            _validationValues = new ValidationValuesDB();
        }

        public async Task<UserFeedsDto> GetFeedingAsync(Guid userFeedID, CancellationToken cancellationToken)
        {
            //var foodsConsumed = await _bd.FoodsConsumed.Where(e => e.userFeedID == userFeedID).ToListAsync(cancellationToken);
            //var foodsConsumedMapped = _mapper.Map<List<FoodsConsumedDto>>(foodsConsumed);

            var userFeeding = await _bd.UserFeeds.FirstOrDefaultAsync(e => e.userFeedID == userFeedID, cancellationToken);
            var userFeedingMapped = _mapper.Map<UserFeedsDto>(userFeeding);

            //userFeedingMapped.foodsConsumed = foodsConsumedMapped;

            return userFeedingMapped;
        }

        public async Task<InfoGeneralFeedingDto> GetInfoGeneralFeedingAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {
            var userFeedingList = await _bd.UserFeeds.Where(e => e.accountID == accountID
                                                            && e.userFeedDate == date).ToListAsync(cancellationToken);


            throw new NotImplementedException();
        }

        public async Task<UserFeedsDto> AddFeedingAsync(AddFeedingDto values, CancellationToken cancellationToken)
        {
            await ExistDailyMeal(values.dailyMeal, cancellationToken);

            var dailyMeal = await _bd.DailyMeals.FirstOrDefaultAsync(e => e.dailyMeal == values.dailyMeal, cancellationToken);

            if (dailyMeal is null) { throw new UnstoredValuesException(); }

            var feedingExisting = await _bd.UserFeeds.FirstOrDefaultAsync(e => e.accountID == values.accountID
                                                                          && e.dailyMealID == dailyMeal.dailyMealID
                                                                          && e.userFeedDate == values.userFeedDate, cancellationToken);

            if (feedingExisting is not null) { throw new RepeatRegistrationException(); }

            var userFeed = CreateUserFeed(values, dailyMeal);

            var foods = await CreateFoods(values.foodsConsumed, cancellationToken);

            var nutritionalValues = await CreateNutritionalValues(foods, values.foodsConsumed, cancellationToken);

            CreateUserFeedNutrValues(userFeed.userFeedID, nutritionalValues, values.foodsConsumed);

            var userFeedingMapped = _mapper.Map<UserFeedsDto>(values);

            userFeedingMapped.userFeedID = userFeed.userFeedID;

            return userFeedingMapped;
        }

        public async Task<UserFeedsDto> UpdateFeedingAsync(UserFeedsDto values, CancellationToken cancellationToken)
        {
            var userFeed = await _bd.UserFeeds.FindAsync(values.userFeedID, cancellationToken);

            if (userFeed is null) { throw new UserFeedNotFoundException(); }

            var dailyMeal = await _bd.DailyMeals.FirstOrDefaultAsync(e => e.dailyMeal == values.dailyMeal, cancellationToken);

            if (dailyMeal is null) { throw new UnstoredValuesException(); }

            if (userFeed.dailyMealID != dailyMeal.dailyMealID)
            {
                userFeed.dailyMealID = dailyMeal.dailyMealID;
            }

            var foods = await CreateFoods(values.foodsConsumed, cancellationToken);

            var nutritionalValues = await CreateNutritionalValues(foods, values.foodsConsumed, cancellationToken);

            await UpdateUserFeedNutrValues(values, nutritionalValues, cancellationToken);

            float totalKcal = TotalKcal(values.foodsConsumed);

            userFeed.satietyLevel = values.satietyLevel;
            userFeed.emotionsLinked = values.emotionsLinked;
            userFeed.totalCalories = totalKcal;
            userFeed.saucerPictureUrl = values.saucerPictureUrl;

            _validationValues.ValidationValues(userFeed);

            if (!Save()) { throw new UnstoredValuesException(); }

            return values;
        }

        public Task<bool> DeleteFeedingAsync(Guid userFeedID, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            try
            {
                return _bd.SaveChanges() >= 0;
            }
            catch (Exception)
            {
                return false;

            }
        }

        private async Task ExistDailyMeal(string dailyMealStr, CancellationToken cancellationToken)
        {
            var existDailyMeal = await _bd.DailyMeals.AnyAsync(e => e.dailyMeal == dailyMealStr, cancellationToken);

            if (!existDailyMeal)
            {
                DailyMeals dailyMeal = new DailyMeals
                {
                    dailyMeal = dailyMealStr
                };

                _validationValues.ValidationValues(dailyMeal);

                _bd.DailyMeals.Add(dailyMeal);

                if (!Save()) { throw new UnstoredValuesException(); }
            }
        }

        private float TotalKcal(List<FoodsConsumedDto> foods)
        {
            return foods.Sum(food => food.nutritionalValues.Sum(e => e.kilocalories));
        }

        private UserFeeds CreateUserFeed(AddFeedingDto values, DailyMeals dailyMeal)
        {
            float totalKcal = TotalKcal(values.foodsConsumed);

            UserFeeds userFeed = new UserFeeds
            {
                accountID = values.accountID,
                dailyMealID = dailyMeal.dailyMealID,
                userFeedDate = values.userFeedDate,
                userFeedTime = values.userFeedTime,
                satietyLevel = values.satietyLevel,
                emotionsLinked = values.emotionsLinked,
                totalCalories = totalKcal,
                saucerPictureUrl = values.saucerPictureUrl
            };

            _validationValues.ValidationValues(userFeed);

            _bd.UserFeeds.Add(userFeed);

            if (!Save()) { throw new UnstoredValuesException(); }

            return userFeed;
        }

        private async Task<Dictionary<string, Guid>> CreateFoods(List<FoodsConsumedDto> foods, CancellationToken cancellationToken)
        {
            var foodCodes = foods.Select(fc => fc.foodCode);

            var existingFoods = await _bd.Foods
                               .Where(f => foodCodes.Contains(f.foodCode))
                               .ToDictionaryAsync(f => f.foodCode, f => f.foodID, cancellationToken);

            var newFoods = foods
                           .Where(fc => !existingFoods.ContainsKey(fc.foodCode))
                           .Select(fc => new Foods
                           {
                               foodCode = fc.foodCode,
                               nameFood = fc.foodName,
                               unit = fc.unit
                           })
                           .ToList();

            if (newFoods.Count > 0)
            {
                newFoods.ForEach(food => _validationValues.ValidationValues(food));

                _bd.Foods.AddRange(newFoods);

                if (!Save()) { throw new UnstoredValuesException(); }
                
                foreach (var newFood in newFoods)
                {
                    existingFoods[newFood.foodCode] = newFood.foodID;
                }
            }

            return existingFoods;
        }

        private async Task<Dictionary<string, Guid>> CreateNutritionalValues(Dictionary<string, Guid> existingFoods, 
                                                                             List<FoodsConsumedDto> foods, 
                                                                             CancellationToken cancellationToken)
        {
            var allNutrValues = foods
                                .SelectMany(nv => nv.nutritionalValues, (nv, nutrValue) => new NutritionalValues
                                {
                                    foodID = existingFoods[nv.foodCode],
                                    nutritionalValueCode = nutrValue.nutritionalValueCode,
                                    portion = nutrValue.portion,
                                    kilocalories = nutrValue.kilocalories,
                                    protein = nutrValue.protein,
                                    carbohydrates = nutrValue.carbohydrates,
                                    totalLipids = nutrValue.totalLipids
                                })
                                .DistinctBy(a => new { a.nutritionalValueCode }) 
                                .ToList();

            var nutrValuesCodes = allNutrValues.Select(nv => nv.nutritionalValueCode);

            var existingNutrValues = await _bd.NutritionalValues
                                     .Where(nv => nutrValuesCodes.Contains(nv.nutritionalValueCode))
                                     .ToDictionaryAsync(nv => nv.nutritionalValueCode, nv => nv.nutritionalValueID, cancellationToken);


            var newNutrValues = allNutrValues
                                .Where(nv => !existingNutrValues.ContainsKey(nv.nutritionalValueCode))
                                .ToList();

            if (newNutrValues.Count > 0)
            {
                newNutrValues.ForEach(nv => _validationValues.ValidationValues(nv));

                _bd.NutritionalValues.AddRange(newNutrValues);

                if (!Save()) { throw new UnstoredValuesException(); }

                foreach (var newNutrValue in newNutrValues)
                {
                    existingNutrValues[newNutrValue.nutritionalValueCode] = newNutrValue.nutritionalValueID;
                }
            }

            return existingNutrValues;
        }

        private void CreateUserFeedNutrValues(Guid userFeedID, Dictionary<string, Guid> existingNutrValues,
                                              List<FoodsConsumedDto> foods)
        {
            var userFeedNutrValues = AllUserFeedNutrValues(userFeedID, foods, existingNutrValues);

            userFeedNutrValues.ForEach(userFeedNutrValue => _validationValues.ValidationValues(userFeedNutrValue));

            _bd.UserFeedNutritionalValues.AddRange(userFeedNutrValues);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private async Task UpdateUserFeedNutrValues(UserFeedsDto values, Dictionary<string, Guid> existingNutrValues, CancellationToken cancellationToken)
        {
            var userFeedNutrValues = AllUserFeedNutrValues(values.userFeedID, values.foodsConsumed, existingNutrValues);

            var existingUserFeedNutrValues = await _bd.UserFeedNutritionalValues
                                             .Where(e => e.userFeedID == values.userFeedID)
                                             .ToListAsync(cancellationToken);

            var newUserFeedNutrValuesDict = userFeedNutrValues.ToDictionary(x => x.nutritionalValueID);
            var existingUserFeedNutrValuesDict = existingUserFeedNutrValues.ToDictionary(x => x.nutritionalValueID);

            var updates = existingUserFeedNutrValues
                          .Where(existing => newUserFeedNutrValuesDict.ContainsKey(existing.nutritionalValueID)
                                && existing.MealFrequency != newUserFeedNutrValuesDict[existing.nutritionalValueID].MealFrequency)
                          .ToList();

            if(updates.Count > 0)
            {
                foreach (var update in updates)
                {
                    update.MealFrequency = newUserFeedNutrValuesDict[update.nutritionalValueID].MealFrequency;
                }
            }

            ToAddition(userFeedNutrValues, existingUserFeedNutrValuesDict);

            ToDelete(existingUserFeedNutrValues, newUserFeedNutrValuesDict);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private static List<UserFeedNutritionalValues> AllUserFeedNutrValues(Guid userFeedID, List<FoodsConsumedDto> foods, Dictionary<string, Guid> existingNutrValues)
        {
            var allNutrValueCodes = foods
                                    .SelectMany(nv => nv.nutritionalValues)
                                    .Select(nutrValue => nutrValue.nutritionalValueCode)
                                    .ToList();

            var userFeedNutrValues = allNutrValueCodes.GroupBy(nutrValueCode => nutrValueCode)
                                     .Select(group => new UserFeedNutritionalValues
                                     {
                                         userFeedID = userFeedID,
                                         nutritionalValueID = existingNutrValues[group.Key],
                                         MealFrequency = group.Count()
                                     })
                                     .ToList();

            return userFeedNutrValues;
        }

        private void ToAddition(List<UserFeedNutritionalValues> userFeedNutrValues, Dictionary<Guid, UserFeedNutritionalValues> existingUserFeedNutrValuesDict)
        {
            var additions = userFeedNutrValues
                            .Where(newItem => !existingUserFeedNutrValuesDict.ContainsKey(newItem.nutritionalValueID))
                            .ToList();

            if (additions.Count > 0)
            {
                _bd.UserFeedNutritionalValues.AddRange(additions);
            }
        }

        private void ToDelete(List<UserFeedNutritionalValues> existingUserFeedNutrValues, Dictionary<Guid, UserFeedNutritionalValues> newUserFeedNutrValuesDict)
        {
            var deletions = existingUserFeedNutrValues
                            .Where(existing => !newUserFeedNutrValuesDict.ContainsKey(existing.nutritionalValueID))
                            .ToList();

            if (deletions.Count > 0)
            {
                _bd.UserFeedNutritionalValues.RemoveRange(deletions);
            }
        }
    }
}
