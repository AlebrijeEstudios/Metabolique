using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Feeding;
using AppVidaSana.Models.Dtos.Feeding_Dtos;
using AppVidaSana.Models.Feeding;
using AppVidaSana.Months_Dates;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;
using AutoMapper;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Security.Cryptography;

namespace AppVidaSana.Services
{
    public class FeedingService : IFeeding
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;
        private const string ContainerName = "storageimages";
        private readonly BlobServiceClient _blobServiceClient;

        public FeedingService(AppDbContext bd, IMapper mapper, BlobServiceClient blobServiceClient)
        {
            _bd = bd;
            _mapper = mapper;
            _blobServiceClient = blobServiceClient;
        }

        public async Task<UserFeedsDto> GetFeedingAsync(Guid userFeedID, CancellationToken cancellationToken)
        {
            var userFeed = await _bd.UserFeeds.FindAsync(new object[] { userFeedID }, cancellationToken);

            var userFeedMapped = _mapper.Map<UserFeedsDto>(userFeed);

            var dailyMeal = await _bd.DailyMeals.FindAsync(new object[] { userFeed!.dailyMealID }, cancellationToken);

            userFeedMapped.dailyMeal = dailyMeal!.dailyMeal;

            var foodsConsumed = await GetFoodsConsumedAsync(userFeedID, cancellationToken);

            var foodsConsumedEnumerated = GetNutrValuesEnumerated(foodsConsumed);

            userFeedMapped.foodsConsumed = foodsConsumedEnumerated;

            userFeedMapped.saucerPictureUrl = await GetSaucerPictureUrlAsync(userFeed.saucerPictureID, cancellationToken);

            return userFeedMapped;
        }

        public async Task<InfoGeneralFeedingDto> GetInfoGeneralFeedingAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {   
            await UpdateCaloriesRequiredPerDays(accountID, date, cancellationToken);

            var kcalConsumed = await GetCaloriesConsumedFeedingsAsync(accountID, date, cancellationToken);

            var userFeeds = await _bd.UserFeeds.Where(e => e.accountID == accountID
                                                      && e.userFeedDate == date).ToListAsync(cancellationToken);


            var dailyMeals = await _bd.DailyMeals
                                      .Where(e => userFeeds.Select(uf => uf.dailyMealID).Contains(e.dailyMealID))
                                      .ToListAsync(cancellationToken);

            CultureInfo cultureInfo = new CultureInfo("es-ES");

            var monthExist = await _bd.Months.FirstOrDefaultAsync(e => e.month == date.ToString("MMMM", cultureInfo)
                                                                  && e.year == Convert.ToInt32(date.ToString("yyyy")),
                                                                  cancellationToken);

            if (monthExist is null)
            {
                return GeneratedInfoGeneralFeeding(userFeeds, dailyMeals, kcalConsumed, false);
            }
            
            var mfuExist = await _bd.MFUsFood.AnyAsync(e => e.accountID == accountID
                                                       && e.monthID == monthExist.monthID, cancellationToken);

            if (!mfuExist)
            {
                return GeneratedInfoGeneralFeeding(userFeeds, dailyMeals, kcalConsumed, false);
            }

            return GeneratedInfoGeneralFeeding(userFeeds, dailyMeals, kcalConsumed, true);
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

            var userFeed = await CreateUserFeed(values, dailyMeal, values.foodsConsumed, cancellationToken);

            var foods = await CreateFoods(values.foodsConsumed, cancellationToken);

            var nutritionalValues = CreateNutritionalValues(foods, values.foodsConsumed);

            CreateUserFeedNutrValues(userFeed.userFeedID, nutritionalValues, values.foodsConsumed);

            await ExistKcalConsumedForDay(values, values.foodsConsumed, cancellationToken);
            
            var userFeedingMapped = _mapper.Map<UserFeedsDto>(values);
            
            userFeedingMapped.userFeedID = userFeed.userFeedID;

            var foodsConsumedEnumerated = GetNutrValuesEnumerated(values.foodsConsumed);

            userFeedingMapped.foodsConsumed = foodsConsumedEnumerated;

            userFeedingMapped.saucerPictureUrl = await GetSaucerPictureUrlAsync(userFeed.saucerPictureID, cancellationToken);

            return userFeedingMapped;
        }

        public async Task<UserFeedsDto> UpdateFeedingAsync(UpdateFeedingDto values, CancellationToken cancellationToken)
        {
            var userFeed = await _bd.UserFeeds.FindAsync(new object[] { values.userFeedID }, cancellationToken);

            if (userFeed is null) { throw new UserFeedNotFoundException(); }

            var dailyMeal = await _bd.DailyMeals.FirstOrDefaultAsync(e => e.dailyMeal == values.dailyMeal, cancellationToken);

            if (dailyMeal is null) { throw new UnstoredValuesException(); }

            if (userFeed.dailyMealID != dailyMeal.dailyMealID)
            {
                userFeed.dailyMealID = dailyMeal.dailyMealID;
            }

            var foods = await CreateFoods(values.foodsConsumed, cancellationToken);

            var nutritionalValues = CreateNutritionalValues(foods, values.foodsConsumed);

            await UpdateUserFeedNutrValues(values, values.foodsConsumed, nutritionalValues, cancellationToken);

            Guid? saucerPictureID = userFeed.saucerPictureID;

            if (values.saucerPicture is not null && values.saucerPicture != "")
            {
                saucerPictureID = await SavePictureAsync(values.saucerPicture, cancellationToken);

                userFeed.saucerPictureID = saucerPictureID;
            }

            if(values.saucerPicture is null)
            {
                saucerPictureID = null;

                userFeed.saucerPictureID = saucerPictureID;
            }

            double totalKcal = TotalKcal(values.foodsConsumed);

            var totalKcalToDate = await _bd.CaloriesConsumed
                                        .FirstOrDefaultAsync(e => e.accountID == userFeed.accountID
                                                             && e.dateCaloriesConsumed == userFeed.userFeedDate,
                                                             cancellationToken);

            var newTotalKcalToDate = (totalKcalToDate!.totalCaloriesConsumed - userFeed.totalCalories) + totalKcal;

            totalKcalToDate.totalCaloriesConsumed = newTotalKcalToDate;

            userFeed.userFeedTime = values.userFeedTime;
            userFeed.satietyLevel = values.satietyLevel;
            userFeed.emotionsLinked = values.emotionsLinked;
            userFeed.totalCalories = totalKcal;

            ValidationValuesDB.ValidationValues(userFeed);

            if (!Save()) { throw new UnstoredValuesException(); }

            var userFeedingMapped = _mapper.Map<UserFeedsDto>(values);

            var foodsConsumedEnumerated = GetNutrValuesEnumerated(values.foodsConsumed);

            userFeedingMapped.foodsConsumed = foodsConsumedEnumerated;

            userFeedingMapped.saucerPictureUrl = await GetSaucerPictureUrlAsync(saucerPictureID, cancellationToken);

            return userFeedingMapped;
        }

        public async Task<bool> DeleteFeedingAsync(Guid userFeedID, CancellationToken cancellationToken)
        {
            float tolerancia = 0.0001f;

            var userFeedToDelete = await _bd.UserFeeds.FindAsync(new object[] { userFeedID }, cancellationToken);

            var totalKcalToDate = await _bd.CaloriesConsumed
                                        .FirstOrDefaultAsync(e => e.accountID == userFeedToDelete!.accountID
                                                             && e.dateCaloriesConsumed == userFeedToDelete.userFeedDate,
                                                             cancellationToken);

            var newTotalKcalToDate = totalKcalToDate!.totalCaloriesConsumed - userFeedToDelete!.totalCalories;

            if(Math.Abs(newTotalKcalToDate) < tolerancia)
            {
                _bd.CaloriesConsumed.Remove(totalKcalToDate);
            }

            if(newTotalKcalToDate > 0)
            {
                totalKcalToDate.totalCaloriesConsumed = newTotalKcalToDate;
            }

            _bd.UserFeeds.Remove(userFeedToDelete);

            if (!Save()) { throw new UnstoredValuesException(); }

            return true;
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

        public List<FoodsConsumedDto> GetNutrValuesEnumerated(List<FoodsConsumedDto> foodsConsumed)
        {
            int number = 0;

            foreach (var food in foodsConsumed)
            {
                var newNutritionalValues = new List<NutritionalValuesDto>();

                foreach (var nv in food.nutritionalValues)
                {
                    var newNv = new NutritionalValuesDto
                    {
                        portion = nv.portion,
                        kilocalories = nv.kilocalories,
                        protein = nv.protein,
                        carbohydrates = nv.carbohydrates,
                        totalLipids = nv.totalLipids
                    };

                    newNv.nutritionalValueCode = (++number).ToString();

                    newNutritionalValues.Add(newNv);
                }

                food.nutritionalValues = newNutritionalValues;
            }

            return foodsConsumed;
        }

        private async Task<UserCalories> CreateUserCaloriesAsync(Guid accountID, CancellationToken cancellationToken)
        {
            var profile = await _bd.Profiles.FindAsync(new object[] { accountID }, cancellationToken);

            double kcalNeeded = 0;

            int age = GetAge(profile!.birthDate);

            if (profile.sex.Equals("Masculino"))
            {
                kcalNeeded = 88.362 + (13.397 * profile.weight) + (4.799 * profile.stature) - (5.677 * age);
            }

            if (profile.sex.Equals("Femenino"))
            {
                kcalNeeded = 447.593 + (9.247 * profile.weight) + (3.098 * profile.stature) - (4.330 * age);
            }

            UserCalories userKcal = new UserCalories
            {
                accountID = profile.accountID,
                caloriesNeeded = kcalNeeded
            };

            ValidationValuesDB.ValidationValues(profile);

            _bd.UserCalories.Add(userKcal);

            if (!Save()) { throw new UnstoredValuesException(); }

            return userKcal;
        }

        private static int GetAge(DateOnly date)
        {
            DateTime dateActual = DateTime.Today;
            int age = dateActual.Year - date.Year;

            if (date.Month > dateActual.Month || (date.Month == dateActual.Month && date.Day > dateActual.Day))
            {
                age--;
            }

            return age;
        }

        private async Task<CaloriesRequiredPerDay> CreateCaloriesRequiredPerDays(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {
            var userKcal = await _bd.UserCalories.FirstOrDefaultAsync(e => e.accountID == accountID, cancellationToken);

            int DayOfWeek = (int) date.DayOfWeek;

            DayOfWeek = DayOfWeek == 0 ? 7 : DayOfWeek;

            DateOnly dateInitial = date.AddDays(-(DayOfWeek - 1));
            DateOnly dateFinal = dateInitial.AddDays(6);

            CaloriesRequiredPerDay kcalRequiredPerDay = new CaloriesRequiredPerDay
            {
                accountID = accountID,
                dateInitial = dateInitial,
                dateFinal = dateFinal,
                caloriesNeeded = userKcal!.caloriesNeeded
            };

            ValidationValuesDB.ValidationValues(kcalRequiredPerDay);

            _bd.CaloriesRequiredPerDays.Add(kcalRequiredPerDay);

            if (!Save()) { throw new UnstoredValuesException(); }

            return kcalRequiredPerDay;
        }

        private async Task<UserCalories> UpdateUserCaloriesAsync(Guid accountID, CancellationToken cancellationToken)
        {
            var profile = await _bd.Profiles.FindAsync(new object[] { accountID }, cancellationToken);

            var userKcal = await _bd.UserCalories.FirstOrDefaultAsync(e => e.accountID == accountID, cancellationToken);

            if(userKcal is null)
            {
                userKcal = await CreateUserCaloriesAsync(accountID, cancellationToken);
                return userKcal;
            }

            double kcalNeeded = 0;

            int age = GetAge(profile!.birthDate);

            if (profile.sex.Equals("Masculino"))
            {
                kcalNeeded = 88.362 + (13.397 * profile.weight) + (4.799 * profile.stature) - (5.677 * age);
            }

            if (profile.sex.Equals("Femenino"))
            {
                kcalNeeded = 447.593 + (9.247 * profile.weight) + (3.098 * profile.stature) - (4.330 * age);
            }

            userKcal.caloriesNeeded = kcalNeeded;

            ValidationValuesDB.ValidationValues(userKcal);

            return userKcal;
        }

        private async Task UpdateCaloriesRequiredPerDays(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {
            var userKcal = await UpdateUserCaloriesAsync(accountID, cancellationToken);

            var kcalRequiredPerDay = await _bd.CaloriesRequiredPerDays
                                              .FirstOrDefaultAsync(e => e.accountID == accountID
                                                                   && e.dateInitial <= date
                                                                   && date <= e.dateFinal, cancellationToken);

            if(kcalRequiredPerDay is null)
            {
                kcalRequiredPerDay = await CreateCaloriesRequiredPerDays(accountID, date, cancellationToken);
            }

            int daysForExercise = await _bd.ActiveMinutes.Where(e => e.accountID == accountID
                                                                && kcalRequiredPerDay.dateInitial <= e.dateExercise
                                                                && e.dateExercise <= kcalRequiredPerDay.dateFinal)
                                                         .CountAsync(cancellationToken);

            if(daysForExercise != 0 && daysForExercise <= 3)
            {
                kcalRequiredPerDay.caloriesNeeded = userKcal!.caloriesNeeded * 1.375;
            }

            if(3 < daysForExercise && daysForExercise <= 5)
            {
                kcalRequiredPerDay.caloriesNeeded = userKcal!.caloriesNeeded * 1.55;
            }

            if(daysForExercise == 6 || daysForExercise == 7)
            {
                kcalRequiredPerDay.caloriesNeeded = userKcal!.caloriesNeeded * 1.725;
            }
            
            int daysExtenuating = await _bd.Exercises.Where(e => e.accountID == accountID 
                                                            && kcalRequiredPerDay.dateInitial <= e.dateExercise
                                                            && e.dateExercise <= kcalRequiredPerDay.dateFinal
                                                            && e.intensityExercise == "Extenuante")
                                                     .Select(e => e.dateExercise)
                                                     .Distinct()
                                                     .CountAsync(cancellationToken);

            if(daysExtenuating == 6 || daysExtenuating == 7)
            {
                kcalRequiredPerDay.caloriesNeeded = userKcal!.caloriesNeeded * 1.9;
            }

            if (daysForExercise == 0) { kcalRequiredPerDay.caloriesNeeded = userKcal!.caloriesNeeded * 1.2; }

            ValidationValuesDB.ValidationValues(kcalRequiredPerDay);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private static InfoGeneralFeedingDto GeneratedInfoGeneralFeeding(List<UserFeeds> userFeeds, List<DailyMeals> dailyMeals,
                                                                         List<CaloriesConsumedFeedingDto> kcalConsumed, bool status)
        {
            InfoGeneralFeedingDto info = new InfoGeneralFeedingDto
            {
                defaultDailyMeals = GetDefaultDailyMeals(userFeeds, dailyMeals),
                othersDailyMeals = GetOthersDailyMeals(userFeeds, dailyMeals),
                caloriesConsumed = kcalConsumed,
                mfuStatus = status
            };

            return info;
        }

        private async Task<List<FoodsConsumedDto>> GetFoodsConsumedAsync(Guid userFeedID, CancellationToken cancellationToken)
        {
            List<FoodsConsumedDto> foodsConsumed = new List<FoodsConsumedDto>();

            var userFeedNutrValues= await _bd.UserFeedNutritionalValues
                                    .Where(e => e.userFeedID == userFeedID)
                                    .ToListAsync(cancellationToken);

            var nutrValues = await _bd.NutritionalValues
                             .Where(e => userFeedNutrValues.Select(ufnv => ufnv.nutritionalValueID).Contains(e.nutritionalValueID))
                             .ToListAsync(cancellationToken);

            var foods = await _bd.Foods
                        .Where(e => nutrValues.Select(nv => nv.foodID).Contains(e.foodID))
                        .Distinct()
                        .ToListAsync(cancellationToken);

            foreach (var food in foods)
            {
                var foodMapped = _mapper.Map<FoodsConsumedDto>(food);

                var nutrValuesForFood = nutrValues
                                        .Where(nv => nv.foodID == food.foodID)
                                        .SelectMany(nv =>
                                        {
                                            var frequency = userFeedNutrValues.FirstOrDefault(ufnv => ufnv.nutritionalValueID == nv.nutritionalValueID)!.MealFrequency;
                                            return Enumerable.Repeat(_mapper.Map<NutritionalValuesDto>(nv), frequency);
                                        })
                                        .ToList();                

                foodMapped.nutritionalValues = nutrValuesForFood;

                foodsConsumed.Add(foodMapped);
            }

            return foodsConsumed;
        }
        
        private static List<StatusDailyMealsDto>? GetDefaultDailyMeals(List<UserFeeds> userFeeds, List<DailyMeals> dailyMeals)
        {
            List<StatusDailyMealsDto>? defaultDailyMeals = new List<StatusDailyMealsDto>();

            string[] dmDefaults = { "Desayuno", "Snack 1", "Comida", "Snack 2", "Cena" };

            defaultDailyMeals.AddRange(
                            dailyMeals.Where(dm => dmDefaults.Contains(dm.dailyMeal))
                                      .SelectMany(dm => userFeeds
                                      .Where(e => e.dailyMealID == dm.dailyMealID)
                                      .Select(uf => new StatusDailyMealsDto
                                      {
                                          userFeedID = uf.userFeedID,
                                          nameDailyMeal = dm.dailyMeal,
                                          dailyMealStatus = true,
                                          totalCalories = uf.totalCalories
                                      })));

            defaultDailyMeals = defaultDailyMeals
                                .OrderBy(dm => Array.IndexOf(dmDefaults, dm.nameDailyMeal))
                                .ToList();

            if(defaultDailyMeals.Count == 0)
            {
                defaultDailyMeals = null;
                return defaultDailyMeals;
            }

            return defaultDailyMeals;
        }

        private static List<StatusDailyMealsDto>? GetOthersDailyMeals(List<UserFeeds> userFeeds, List<DailyMeals> dailyMeals)
        {
            List<StatusDailyMealsDto>? othersDailyMeals = new List<StatusDailyMealsDto>();

            string[] dmDefaults = { "Desayuno", "Snack 1", "Comida", "Snack 2", "Cena" };

            othersDailyMeals.AddRange(
                           dailyMeals.Where(dm => !dmDefaults.Contains(dm.dailyMeal))
                                     .SelectMany(dm => userFeeds
                                     .Where(e => e.dailyMealID == dm.dailyMealID)
                                     .Select(uf => new StatusDailyMealsDto
                                     {
                                         userFeedID = uf.userFeedID,
                                         nameDailyMeal = dm.dailyMeal,
                                         dailyMealStatus = true,
                                         totalCalories = uf.totalCalories
                                     })));

            othersDailyMeals = othersDailyMeals
                               .OrderBy(dm =>
                               {
                                   var numbers = new string(dm.nameDailyMeal.Where(char.IsDigit).ToArray());
                                   return int.Parse(numbers);
                               })
                               .ToList();

            if (othersDailyMeals.Count == 0)
            {
                othersDailyMeals = null;
                return othersDailyMeals;
            }

            return othersDailyMeals;
        }

        private async Task<List<CaloriesConsumedFeedingDto>> GetCaloriesConsumedFeedingsAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {
            var limit = await _bd.CaloriesRequiredPerDays.FirstOrDefaultAsync(e => e.accountID == accountID
                                                                              && e.dateInitial <= date
                                                                              && date <= e.dateFinal, cancellationToken);

            int DayOfWeek = (int)date.DayOfWeek;

            DayOfWeek = DayOfWeek == 0 ? 7 : DayOfWeek;

            DateOnly dateInitial = date.AddDays(-(DayOfWeek - 1));
            DateOnly dateFinal = dateInitial.AddDays(6);

            var dates = DatesInRange.GetDatesInRange(dateInitial, dateFinal);

            if (limit is not null)
            {
                dates = DatesInRange.GetDatesInRange(limit.dateInitial, limit.dateFinal);
            }

            var values = await _bd.CaloriesConsumed
                                  .Where(c => c.accountID == accountID
                                         && dates.Contains(c.dateCaloriesConsumed)).ToListAsync(cancellationToken);

            var kcalConsumedFeedings = dates.Select(date => new CaloriesConsumedFeedingDto
                                            {
                                                date = date,
                                                limit = limit?.caloriesNeeded ?? 0,
                                                value = values.FirstOrDefault(e => e.dateCaloriesConsumed == date)?.totalCaloriesConsumed ?? 0
                                            }).ToList();

            return kcalConsumedFeedings;
        }

        private async Task<string?> GetSaucerPictureUrlAsync(Guid? saucerPictureID, CancellationToken cancellationToken)
        {
            if (saucerPictureID is null)
            {
                return null;
            }

            var suacerPictureUrl = await _bd.SaucerPictures.FindAsync(new object[] { saucerPictureID }, cancellationToken);
            return suacerPictureUrl!.saucerPictureUrl;
        }

        private static double TotalKcal(List<FoodsConsumedDto> foods)
        {
            return foods.Select(food => food.nutritionalValues.Sum(e => e.kilocalories)).Sum();
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

                ValidationValuesDB.ValidationValues(dailyMeal);

                _bd.DailyMeals.Add(dailyMeal);

                if (!Save()) { throw new UnstoredValuesException(); }
            }
        }

        private async Task ExistKcalConsumedForDay(AddFeedingDto values, List<FoodsConsumedDto> foodsConsumed, CancellationToken cancellationToken)
        {
            var kcalConsumedExist = await _bd.CaloriesConsumed.FirstOrDefaultAsync(e => e.accountID == values.accountID
                                                                                   && e.dateCaloriesConsumed == values.userFeedDate,
                                                                                   cancellationToken);

            if (kcalConsumedExist is not null) {

                kcalConsumedExist.totalCaloriesConsumed = kcalConsumedExist.totalCaloriesConsumed + TotalKcal(foodsConsumed);
            }

            if(kcalConsumedExist is null)
            {
                CaloriesConsumed kcalConsumed = new CaloriesConsumed
                {
                    accountID = values.accountID,
                    dateCaloriesConsumed = values.userFeedDate,
                    totalCaloriesConsumed = TotalKcal(foodsConsumed)
                };

                ValidationValuesDB.ValidationValues(kcalConsumed);

                _bd.CaloriesConsumed.Add(kcalConsumed);
            }

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private async Task<UserFeeds> CreateUserFeed(AddFeedingDto values, DailyMeals dailyMeal, List<FoodsConsumedDto> foodsConsumed, CancellationToken cancellationToken)
        {
            double totalKcal = TotalKcal(foodsConsumed);

            Guid? saucerPictureID = null;

            if(values.saucerPicture is not null)
            {
                saucerPictureID = await SavePictureAsync(values.saucerPicture, cancellationToken);
            }

            UserFeeds userFeed = new UserFeeds
            {
                accountID = values.accountID,
                dailyMealID = dailyMeal.dailyMealID,
                userFeedDate = values.userFeedDate,
                userFeedTime = values.userFeedTime,
                satietyLevel = values.satietyLevel,
                emotionsLinked = values.emotionsLinked,
                totalCalories = totalKcal,
                saucerPictureID = saucerPictureID
            };

            ValidationValuesDB.ValidationValues(userFeed);

            _bd.UserFeeds.Add(userFeed);

            if (!Save()) { throw new UnstoredValuesException(); }

            return userFeed;
        }

        private BlobContainerClient GetContainerClient()
        {
            BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient(ContainerName);
            containerClient.CreateIfNotExists();

            return containerClient;
        }

        private async Task<Guid> SavePictureAsync(string picture, CancellationToken cancellationToken)
        {
            try
            {
                string cleanBase64 = picture.Split(',')[1];

                var imageBytes = Convert.FromBase64String(cleanBase64);

                var hashPicture = GetImageHashSHA256(imageBytes);

                var fileUrl = await _bd.SaucerPictures.FirstOrDefaultAsync(e => e.hashPicture == hashPicture, cancellationToken);

                if(fileUrl is null)
                {
                    var namePicture = GetNameToImage(picture);

                    var blobClient = GetContainerClient().GetBlobClient(namePicture);

                    var mimeType = GetMimeType(namePicture);

                    var httpHeaders = new BlobHttpHeaders
                    {
                        ContentType = mimeType
                    };

                    var uploadOptions = new BlobUploadOptions
                    {
                        HttpHeaders = httpHeaders
                    };

                    await blobClient.UploadAsync(new MemoryStream(imageBytes), uploadOptions, cancellationToken);
                    var url = blobClient.Uri.ToString();

                    SaucerPictures saucerPicture = new SaucerPictures
                    {
                        hashPicture = hashPicture,
                        saucerPictureUrl = url
                    };

                    ValidationValuesDB.ValidationValues(saucerPicture);

                    _bd.SaucerPictures.Add(saucerPicture);

                    if (!Save()) { throw new UnstoredValuesException(); }

                    return saucerPicture.saucerPictureID;
                }

                return fileUrl.saucerPictureID;
            }
            catch(Exception)
            {
                throw new UnstoredValuesException();
            }
        }

        private static string GetImageHashSHA256(byte[] imageBytes)
        {
            byte[] imageToHash = SHA256.HashData(imageBytes);

            return BitConverter.ToString(imageToHash).Replace("-", "").ToLower();
        }

        private static string GetNameToImage(string base64Image)
        {
            string name = "";

            if (base64Image.StartsWith("data:image/jpg")) { name = $"{Guid.NewGuid()}.jpg"; }
            if (base64Image.StartsWith("data:image/jpeg")) { name = $"{Guid.NewGuid()}.jpeg"; }
            if (base64Image.StartsWith("data:image/png")) { name = $"{Guid.NewGuid()}.png"; }
            if (base64Image.StartsWith("data:image/webp")) { name = $"{Guid.NewGuid()}.webp"; }

            return name;
        }

        private static string GetMimeType(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLowerInvariant();
            return extension switch
            {
                ".jpg" => "image/jpeg",
                ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".webp" => "image/webp",
                _ => "application/octet-stream"
            };
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
                               nameFood = fc.nameFood,
                               unit = fc.unit
                           })
                           .ToList();

            if (newFoods.Count > 0)
            {
                newFoods.ForEach(food => ValidationValuesDB.ValidationValues(food));

                _bd.Foods.AddRange(newFoods);

                if (!Save()) { throw new UnstoredValuesException(); }
                
                foreach (var newFood in newFoods)
                {
                    existingFoods[newFood.foodCode] = newFood.foodID;
                }
            }

            return existingFoods;
        }

        private List<NutritionalValues> CreateNutritionalValues(Dictionary<string, Guid> existingFoods, 
                                                                List<FoodsConsumedDto> foods)
        {
            float tolerance = 0.0001f;

            var allNutrValues = foods
                                .SelectMany(food => food.nutritionalValues
                                .GroupBy(nutrValue => new
                                {
                                    nutrValue.portion,
                                    nutrValue.kilocalories,
                                    nutrValue.protein,
                                    nutrValue.carbohydrates,
                                    nutrValue.totalLipids
                                })
                                .Select(group => group.First())
                                .Select(nutrValue => new NutritionalValues
                                {
                                    foodID = existingFoods[food.foodCode],
                                    portion = nutrValue.portion,
                                    kilocalories = nutrValue.kilocalories,
                                    protein = nutrValue.protein,
                                    carbohydrates = nutrValue.carbohydrates,
                                    totalLipids = nutrValue.totalLipids
                                })
                                ).ToList();

            var foodIDs = allNutrValues.Select(nv => nv.foodID);

            var existingNutrValues = _bd.NutritionalValues
                                    .Where(nv => foodIDs.Contains(nv.foodID)) 
                                    .AsEnumerable() 
                                    .Where(nv => allNutrValues.Any(anv =>
                                        anv.foodID == nv.foodID &&
                                        anv.portion == nv.portion &&
                                        Math.Abs(anv.kilocalories - nv.kilocalories) < tolerance &&
                                        Math.Abs(anv.protein - nv.protein) < tolerance &&
                                        Math.Abs(anv.carbohydrates - nv.carbohydrates) < tolerance &&
                                        Math.Abs(anv.totalLipids - nv.totalLipids) < tolerance))
                                    .ToList();

            var newNutrValues = allNutrValues
                                .Where(nv => !existingNutrValues.Any(anv =>
                                    anv.foodID == nv.foodID &&
                                    anv.portion == nv.portion &&
                                    Math.Abs(anv.kilocalories - nv.kilocalories) < tolerance &&
                                    Math.Abs(anv.protein - nv.protein) < tolerance &&
                                    Math.Abs(anv.carbohydrates - nv.carbohydrates) < tolerance &&
                                    Math.Abs(anv.totalLipids - nv.totalLipids) < tolerance))
                                .ToList();

            if (newNutrValues.Count > 0)
            {
                newNutrValues.ForEach(nv => ValidationValuesDB.ValidationValues(nv));

                _bd.NutritionalValues.AddRange(newNutrValues);

                if (!Save()) { throw new UnstoredValuesException(); }

                existingNutrValues.AddRange(newNutrValues);
            }

            return existingNutrValues;
        }

        private void CreateUserFeedNutrValues(Guid userFeedID, List<NutritionalValues> existingNutrValues, List<FoodsConsumedDto> foods)
        {
            var userFeedNutrValues = AllUserFeedNutrValues(userFeedID, foods, existingNutrValues);

            userFeedNutrValues.ForEach(userFeedNutrValue => ValidationValuesDB.ValidationValues(userFeedNutrValue));

            _bd.UserFeedNutritionalValues.AddRange(userFeedNutrValues);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private async Task UpdateUserFeedNutrValues(UpdateFeedingDto values, List<FoodsConsumedDto> foodsConsumed,
                                                    List<NutritionalValues> existingNutrValues, CancellationToken cancellationToken)
        {
            var userFeedNutrValues = AllUserFeedNutrValues(values.userFeedID, foodsConsumed, existingNutrValues);

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

        private static List<UserFeedNutritionalValues> AllUserFeedNutrValues(Guid userFeedID, 
                                                                             List<FoodsConsumedDto> foods,
                                                                             List<NutritionalValues> existingNutrValues)
        {
            float tolerance = 0.0001f;

            var allNutrValueCodes = foods
                                    .SelectMany(nv => nv.nutritionalValues)
                                    .Select(nutrValue => new
                                    {
                                        nutrValue.portion,
                                        nutrValue.kilocalories,
                                        nutrValue.protein,
                                        nutrValue.carbohydrates,
                                        nutrValue.totalLipids
                                    }).ToList();

            var userFeedNutrValues = allNutrValueCodes.GroupBy(nutrValue => nutrValue)
                                     .Select(group => new UserFeedNutritionalValues
                                     {
                                         userFeedID = userFeedID,
                                         nutritionalValueID = existingNutrValues
                                                              .FirstOrDefault(e => e.portion == group.Key.portion &&
                                                                              Math.Abs(e.kilocalories - group.Key.kilocalories) < tolerance &&
                                                                              Math.Abs(e.protein - group.Key.protein) < tolerance &&
                                                                              Math.Abs(e.carbohydrates - group.Key.carbohydrates) < tolerance &&
                                                                              Math.Abs(e.totalLipids - group.Key.totalLipids) < tolerance)!.nutritionalValueID,
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
