using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Feeding;
using AppVidaSana.Models.Dtos.Feeding_Dtos;
using AppVidaSana.Models.Feeding;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;

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
            List<Foods> foods = new List<Foods>();
            List<NutritionalValues> nutritionalValues = new List<NutritionalValues>();
            List<UserFeedNutritionalValues> userFeedNutrValues = new List<UserFeedNutritionalValues>();

            await ExistDailyMeal(values.dailyMeal, cancellationToken);

            var dailyMeal = await _bd.DailyMeals.FirstOrDefaultAsync(e => e.dailyMeal == values.dailyMeal, cancellationToken);

            if (dailyMeal is null) { throw new UnstoredValuesException(); }

            var feedingExisting = await _bd.UserFeeds.FirstOrDefaultAsync(e => e.accountID == values.accountID
                                                                          && e.dailyMealID == dailyMeal.dailyMealID
                                                                          && e.userFeedDate == values.userFeedDate, cancellationToken);

            if (feedingExisting is not null) { throw new RepeatRegistrationException(); }

            var userFeed = CreateUserFeed(values, dailyMeal);

            foreach (var food in values.foodsConsumed)
            {
                var foodObj = CreateFood(food);

                nutritionalValues = CreateNutritionalValues(foodObj.foodID, food.nutritionalValues);

                userFeedNutrValues = CreateUserFeedNutrValues(userFeed.userFeedID, nutritionalValues);

                nutritionalValues = WithoutRepetitions(nutritionalValues);

                foods.Add(foodObj);
            }

            await AddFoods(foods, cancellationToken);

            await AddNutritionalValues(nutritionalValues, cancellationToken);

            AddUserFeed(userFeed);
            
            await AddCreateUserFeedNutrValues(userFeedNutrValues, cancellationToken);

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

            userFeed.satietyLevel = values.satietyLevel;
            userFeed.emotionsLinked = values.emotionsLinked;
            userFeed.saucerPictureUrl = values.saucerPictureUrl;

            _validationValues.ValidationValues(userFeed);

            _bd.UserFeeds.Update(userFeed);

            if (!Save()) { throw new UnstoredValuesException(); }

            //await UpdateFoodsConsumedAsync(userFeed.userFeedID, values.foodsConsumed, cancellationToken);

            var userFeedingMapped = _mapper.Map<UserFeedsDto>(userFeed);

            userFeedingMapped.foodsConsumed = values.foodsConsumed;

            return userFeedingMapped;
        }

        public Task<bool> DeleteFeedingAsync(Guid userFeedID, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
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

        private UserFeeds CreateUserFeed(AddFeedingDto values, DailyMeals dailyMeal)
        {
            float totalKcal = values.foodsConsumed.Sum(food => food.nutritionalValues.Sum(e => e.kilocalories));

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

            return userFeed;
        }

        private static Foods CreateFood(FoodsConsumedDto foods)
        {
            Foods food = new Foods
            {
                foodCode = foods.foodCode,
                nameFood = foods.foodName,
                unit = foods.unit
            };

            return food;
        }

        private static List<NutritionalValues> CreateNutritionalValues(Guid foodID, List<NutritionalValuesDto> values)
        {
            List<NutritionalValues> nutritionalValues = new List<NutritionalValues>();

            values.ForEach(value => nutritionalValues.Add(new NutritionalValues
                                    {
                                        foodID = foodID,
                                        nutritionalValueCode = value.nutritionalValueCode,
                                        portion = value.portion,
                                        kilocalories = value.kilocalories,
                                        protein = value.protein,
                                        carbohydrates = value.carbohydrates,
                                        totalLipids = value.totalLipids
                                    }));

            return nutritionalValues;
        }

        private static List<UserFeedNutritionalValues> CreateUserFeedNutrValues(Guid userFeedID, List<NutritionalValues> values)
        { 
            return values.GroupBy(c => c.nutritionalValueID)
                         .Select(group => new UserFeedNutritionalValues
                         {
                            userFeedID = userFeedID,
                            nutritionalValueID = group.Key,
                            MealFrequency = group.Count()
                         })
                         .ToList();
        }

        private static List<NutritionalValues> WithoutRepetitions(List<NutritionalValues> values)
        {
            return values.Distinct().ToList();
        }

        private void AddUserFeed(UserFeeds values)
        {
            _validationValues.ValidationValues(values);

            _bd.UserFeeds.Add(values);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private async Task AddFoods(List<Foods> values, CancellationToken cancellationToken)
        {
            var existFoods = await _bd.Foods
                             .Where(f => values.Contains(f)) 
                             .Select(f => f)               
                             .ToListAsync(cancellationToken);

            var missingFoods = values.Except(existFoods).ToList();

            if(missingFoods.Count > 0)
            {
                missingFoods.ForEach(food => _validationValues.ValidationValues(food));

                _bd.Foods.AddRange(missingFoods);

                if (!Save()) { throw new UnstoredValuesException(); }
            }
        }

        private async Task AddNutritionalValues(List<NutritionalValues> values, CancellationToken cancellationToken)
        {
            var existNutrValues = await _bd.NutritionalValues
                                  .Where(nv => values.Contains(nv))
                                  .Select(nv => nv)
                                  .ToListAsync(cancellationToken);

            var missingNutrValues = values.Except(existNutrValues).ToList();

            if(missingNutrValues.Count > 0)
            {
                missingNutrValues.ForEach(nutrValue => _validationValues.ValidationValues(nutrValue));

                _bd.NutritionalValues.AddRange(missingNutrValues);

                if (!Save()) { throw new UnstoredValuesException(); }
            }
        }

        private async Task AddCreateUserFeedNutrValues(List<UserFeedNutritionalValues> values, CancellationToken cancellationToken)
        {
            var existUserFeedNutrValues = await _bd.UserFeedNutritionalValues
                                          .Where(ufnv => values.Contains(ufnv))
                                          .Select(ufnv => ufnv)
                                          .ToListAsync(cancellationToken);

            var missingUserFeedNutrValues = values.Except(existUserFeedNutrValues).ToList();

            if(missingUserFeedNutrValues.Count > 0)
            {
                missingUserFeedNutrValues.ForEach(userFeedNutrValue => _validationValues.ValidationValues(userFeedNutrValue));

                _bd.UserFeedNutritionalValues.AddRange(missingUserFeedNutrValues);

                if (!Save()) { throw new UnstoredValuesException(); }
            }
        }


        /*private async Task UpdateFoodsConsumedAsync(Guid userFeedID, List<FoodsConsumedDto> foods, CancellationToken cancellationToken)
        {
            var foodsToEliminate = await _bd.FoodsConsumed.Where(e => e.userFeedID == userFeedID).ToListAsync(cancellationToken);

            _bd.FoodsConsumed.RemoveRange(foodsToEliminate);

            if (!Save()) { throw new UnstoredValuesException(); }

            AddFoodsConsumed(userFeedID, foods);
        }*/
    }
}
