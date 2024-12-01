using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Feeding;
using AppVidaSana.GraphicValues;
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
                                                                          && e.userFeedDate == values.userFeedDate
                                                                          && e.userFeedTime == values.userFeedTime
                                                                          && e.satietyLevel == values.satietyLevel
                                                                          && e.emotionsLinked == values.emotionsLinked
                                                                          && e.saucerPictureUrl == values.saucerPictureUrl, cancellationToken);

            if (feedingExisting is not null) { throw new RepeatRegistrationException(); }

            UserFeeds userFeed = new UserFeeds
            {
                accountID = values.accountID,
                dailyMealID = dailyMeal.dailyMealID,
                userFeedDate = values.userFeedDate,
                userFeedTime = values.userFeedTime,
                satietyLevel = values.satietyLevel,
                emotionsLinked = values.emotionsLinked,
                saucerPictureUrl = values.saucerPictureUrl
            };

            _validationValues.ValidationValues(userFeed);

            _bd.UserFeeds.Add(userFeed);

            if (!Save()) { throw new UnstoredValuesException(); }

            //AddFoodsConsumed(userFeed.userFeedID, values.foodsConsumed);

            var userFeedingMapped = _mapper.Map<UserFeedsDto>(userFeed);

            userFeedingMapped.foodsConsumed = values.foodsConsumed;

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

                _bd.DailyMeals.Add(dailyMeal);

                if (!Save()) { throw new UnstoredValuesException(); }
            }
        }

        /*private void AddFoodsConsumed(Guid userFeedID, List<FoodsConsumedDto> foods)
        {
            var foodsConsumed = _mapper.Map<List<FoodConsumed>>(foods);

            foodsConsumed.ForEach(food => food.userFeedID = userFeedID);

            foodsConsumed.ForEach(food => _validationValues.ValidationValues(food));

            _bd.FoodsConsumed.AddRange(foodsConsumed);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private async Task UpdateFoodsConsumedAsync(Guid userFeedID, List<FoodsConsumedDto> foods, CancellationToken cancellationToken)
        {
            var foodsToEliminate = await _bd.FoodsConsumed.Where(e => e.userFeedID == userFeedID).ToListAsync(cancellationToken);

            _bd.FoodsConsumed.RemoveRange(foodsToEliminate);

            if (!Save()) { throw new UnstoredValuesException(); }

            AddFoodsConsumed(userFeedID, foods);
        }*/
    }
}
