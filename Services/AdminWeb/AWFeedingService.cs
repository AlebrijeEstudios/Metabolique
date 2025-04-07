using AppVidaSana.Data;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.Models.Dtos.Feeding_Dtos;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWFeedingService : IAWFeeding
    {
        private readonly AppDbContext _bd;

        public AWFeedingService(AppDbContext bd)
        {
            _bd = bd;
        }

        public async Task<List<FoodsConsumedAdminDto>> GetFeedingsAsync(Guid accountID, int page, CancellationToken cancellationToken)
        {
            var feedings = await _bd.UserFeeds
                            .Where(e => e.accountID == accountID)
                            .Include(f => f.dailyMeals)
                            .Include(f => f.saucerPicture)
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync(cancellationToken);

            var feedingDTOs = feedings.Select(feeding => new FoodsConsumedAdminDto
            {
                userFeedID = feeding.userFeedID,
                userFeedDate = feeding.userFeedDate,
                userFeedTime = feeding.userFeedTime,
                dailyMeal = feeding.dailyMeals?.dailyMeal ?? "N/A",
                foodsConsumed = [],
                satietyLevel = feeding.satietyLevel,
                emotionsLinked = feeding.emotionsLinked,
                totalCalories = feeding.totalCalories,
                saucerPictureUrl = feeding.saucerPicture?.saucerPictureUrl
            }).ToList();

            return feedingDTOs;
        }

        public async Task<List<FoodsConsumedAdminDto>> GetFilterFeedingsAsync(Guid accountID, int page, DateOnly dateInitial, DateOnly dateFinal, CancellationToken cancellationToken)
        {
            var feedings = await _bd.UserFeeds
                            .Where(e => e.accountID == accountID &&
                                        e.userFeedDate >= dateInitial &&
                                        e.userFeedDate <= dateFinal
                            )
                            .Include(f => f.dailyMeals)
                            .Include(f => f.saucerPicture)
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync(cancellationToken);

            var feedingDTOs = feedings.Select(feeding => new FoodsConsumedAdminDto
            {
                userFeedID = feeding.userFeedID,
                userFeedDate = feeding.userFeedDate,
                userFeedTime = feeding.userFeedTime,
                dailyMeal = feeding.dailyMeals?.dailyMeal ?? "N/A",
                foodsConsumed = [],
                satietyLevel = feeding.satietyLevel,
                emotionsLinked = feeding.emotionsLinked,
                totalCalories = feeding.totalCalories,
                saucerPictureUrl = feeding.saucerPicture?.saucerPictureUrl
            }).ToList();

            return feedingDTOs;
        }

        public async Task<byte[]> ExportFeedingsAsync(Guid accountID, CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("UserFeedID,UserFeedDate,UserFeedTime,DailyMeal,FoodsConsumed,SatietyLevel,EmotionsLinked,TotalCalories,SaucerPictureUrl");

                while (currentPage >= 0)
                {
                    var feedings = await _bd.UserFeeds
                            .Where(e => e.accountID == accountID)
                            .Include(f => f.dailyMeals)
                            .Include(f => f.saucerPicture)
                            .Include(f => f.userFeedNutritionalValues) 
                            .ThenInclude(nv => nv.nutritionalValues) 
                            .ThenInclude(nv => nv.foods)
                            .OrderBy(f => f.userFeedID)
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    var feedingDTOs = feedings.Select(feeding => new FoodsConsumedAdminDto
                    {
                        userFeedID = feeding.userFeedID,
                        userFeedDate = feeding.userFeedDate,
                        userFeedTime = feeding.userFeedTime,
                        dailyMeal = feeding.dailyMeals?.dailyMeal ?? "N/A",
                        foodsConsumed = feeding.userFeedNutritionalValues
                                        .Select(nv => new FoodsConsumedDto
                                        {
                                            foodCode = nv.nutritionalValues.foods.foodCode,
                                            nameFood = nv.nutritionalValues.foods.nameFood,
                                            unit = nv.nutritionalValues.foods.unit,
                                            nutritionalValues = new List<NutritionalValuesDto>
                                            {
                                                new NutritionalValuesDto
                                                {
                                                    nutritionalValueCode = nv.nutritionalValues.nutritionalValueID.ToString(),
                                                    portion = nv.nutritionalValues.portion,
                                                    kilocalories = nv.nutritionalValues.kilocalories,
                                                    protein = nv.nutritionalValues.protein,
                                                    carbohydrates = nv.nutritionalValues.carbohydrates,
                                                    totalLipids = nv.nutritionalValues.totalLipids
                                                }
                                            }
                                        })
                                        .ToList(),
                        satietyLevel = feeding.satietyLevel,
                        emotionsLinked = feeding.emotionsLinked,
                        totalCalories = feeding.totalCalories,
                        saucerPictureUrl = feeding.saucerPicture?.saucerPictureUrl
                    }).ToList();

                    if (feedingDTOs.Count == 0)
                    {
                        break;
                    }

                    foreach (var feeding in feedingDTOs)
                    {
                        var foodsConsumedDetails = feeding.foodsConsumed.Select(f =>
                                                    $"(FoodCode: {f.foodCode}, NameFood: {f.nameFood}, Unit: {f.unit}, Portion: {f.nutritionalValues.First().portion}, " +
                                                    $"Kcal: {f.nutritionalValues.First().kilocalories}, Protein: {f.nutritionalValues.First().protein}, " +
                                                    $"Carbohydrates: {f.nutritionalValues.First().carbohydrates}, TotalLipids: {f.nutritionalValues.First().totalLipids})"
                                                ).ToList();

                        var foodsConsumedString = string.Join("; ", foodsConsumedDetails);

                        var csvLine = $"{feeding.userFeedID},{feeding.userFeedDate},{feeding.userFeedTime},{feeding.dailyMeal ?? "N/A"}," +
                                      $"\"{foodsConsumedString}\"," + 
                                      $"{feeding.satietyLevel},\"{feeding.emotionsLinked}\",{feeding.totalCalories},{feeding.saucerPictureUrl ?? "N/A"}";
                        
                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportFilteredFeedingsAsync(Guid accountID, DateOnly dateInitial, DateOnly dateFinal, CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("UserFeedID,UserFeedDate,UserFeedTime,DailyMeal,SatietyLevel,EmotionsLinked,TotalCalories,SaucerPictureUrl");

                while (currentPage >= 0)
                {
                    var feedings = await _bd.UserFeeds
                            .Where(e => e.accountID == accountID &&
                                        e.userFeedDate >= dateInitial &&
                                        e.userFeedDate <= dateFinal)
                            .Include(f => f.dailyMeals)
                            .Include(f => f.saucerPicture)
                            .OrderBy(f => f.userFeedID)
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    var feedingDTOs = feedings.Select(feeding => new FeedingsAdminDto
                    {
                        userFeedID = feeding.userFeedID,
                        userFeedDate = feeding.userFeedDate,
                        userFeedTime = feeding.userFeedTime,
                        dailyMeal = feeding.dailyMeals?.dailyMeal ?? "N/A",
                        satietyLevel = feeding.satietyLevel,
                        emotionsLinked = feeding.emotionsLinked,
                        totalCalories = feeding.totalCalories,
                        saucerPictureUrl = feeding.saucerPicture?.saucerPictureUrl
                    }).ToList();

                    if (feedingDTOs.Count == 0)
                    {
                        break;
                    }

                    foreach (var feeding in feedingDTOs)
                    {
                        var csvLine = $"{feeding.userFeedID},{feeding.userFeedDate},{feeding.userFeedTime},{feeding.dailyMeal ?? "N/A"},{feeding.satietyLevel},\"{feeding.emotionsLinked}\",{feeding.totalCalories},{feeding.saucerPictureUrl ?? "N/A"}";
                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllFoodsConsumedPerFeedingAsync(CancellationToken cancellationToken) 
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("UserFeedID,UserFeedDate,UserFeedTime,DailyMeal,FoodCode,NameFood,Unit,Portion,Carbohydrates,Protein,TotalLipids,Kcal");

                while (currentPage >= 0)
                {
                    var feedings = await _bd.UserFeeds
                            .Include(f => f.dailyMeals)
                            .Include(f => f.saucerPicture)
                            .Include(f => f.userFeedNutritionalValues)
                            .ThenInclude(nv => nv.nutritionalValues)
                            .ThenInclude(nv => nv.foods)
                            .OrderBy(f => f.userFeedID)
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    var feedingDTOs = feedings.Select(feeding => new FoodsConsumedAdminDto
                    {
                        userFeedID = feeding.userFeedID,
                        userFeedDate = feeding.userFeedDate,
                        userFeedTime = feeding.userFeedTime,
                        dailyMeal = feeding.dailyMeals?.dailyMeal ?? "N/A",
                        foodsConsumed = feeding.userFeedNutritionalValues
                                        .Select(nv => new FoodsConsumedDto
                                        {
                                            foodCode = nv.nutritionalValues.foods.foodCode,
                                            nameFood = nv.nutritionalValues.foods.nameFood,
                                            unit = nv.nutritionalValues.foods.unit,
                                            nutritionalValues = Enumerable.Repeat(
                                                new NutritionalValuesDto
                                                {
                                                    nutritionalValueCode = nv.nutritionalValues.nutritionalValueID.ToString(),
                                                    portion = nv.nutritionalValues.portion,
                                                    kilocalories = nv.nutritionalValues.kilocalories,
                                                    protein = nv.nutritionalValues.protein,
                                                    carbohydrates = nv.nutritionalValues.carbohydrates,
                                                    totalLipids = nv.nutritionalValues.totalLipids
                                                }, nv.MealFrequency
                                            ).ToList()
                                        })
                                        .ToList(),
                        satietyLevel = feeding.satietyLevel,
                        emotionsLinked = feeding.emotionsLinked,
                        totalCalories = feeding.totalCalories,
                        saucerPictureUrl = feeding.saucerPicture?.saucerPictureUrl
                    }).ToList();

                    if (feedingDTOs.Count == 0)
                    {
                        break;
                    }

                    foreach (var feeding in feedingDTOs)
                    {
                        foreach (var fC in feeding.foodsConsumed) 
                        {
                            foreach(var nV in fC.nutritionalValues) 
                            { 
                                var csvLine = $"{feeding.userFeedID},{feeding.userFeedDate},{feeding.userFeedTime},{feeding.dailyMeal ?? "N/A"}," +
                                          $"{fC.foodCode},\"{fC.nameFood}\",{fC.unit},' {nV.portion}',{nV.carbohydrates},{nV.protein},{nV.totalLipids},{nV.kilocalories}";

                                await streamWriter.WriteLineAsync(csvLine);
                            }
                        }
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllFeedingsAsync(CancellationToken cancellationToken) 
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("AccountID,UserFeedID,Username,UserFeedDate,UserFeedTime,DailyMeal,TotalCarbohydrates,TotalProtein,TotalLipids,TotalCalories,SatietyLevel,EmotionsLinked,SaucerPictureUrl");

                while (currentPage >= 0)
                {
                    var feedings = await _bd.UserFeeds
                            .Include(f => f.account)
                            .Include(f => f.dailyMeals)
                            .Include(f => f.saucerPicture)
                            .Include(f => f.userFeedNutritionalValues)
                            .ThenInclude(nv => nv.nutritionalValues)
                            .ThenInclude(nv => nv.foods)
                            .OrderBy(f => f.userFeedID)
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    var feedingDTOs = feedings.Select(feeding => new FeedingsAdminDto
                    {
                        accountID = feeding.account.accountID,
                        userFeedID = feeding.userFeedID, 
                        userName = feeding.account.username,
                        userFeedDate = feeding.userFeedDate,
                        userFeedTime = feeding.userFeedTime,
                        dailyMeal = feeding.dailyMeals?.dailyMeal ?? "N/A",
                        totalCarbohydrates = feeding.userFeedNutritionalValues
                                             .Sum(nv => nv.nutritionalValues.carbohydrates * nv.MealFrequency),
                        totalProtein = feeding.userFeedNutritionalValues
                                             .Sum(nv => nv.nutritionalValues.protein * nv.MealFrequency),
                        totalLipids = feeding.userFeedNutritionalValues
                                             .Sum(nv => nv.nutritionalValues.totalLipids * nv.MealFrequency),
                        totalCalories = feeding.totalCalories,
                        satietyLevel = feeding.satietyLevel,
                        emotionsLinked = feeding.emotionsLinked,
                        saucerPictureUrl = feeding.saucerPicture?.saucerPictureUrl
                    }).ToList();

                    if (feedingDTOs.Count == 0)
                    {
                        break;
                    }

                    foreach (var feeding in feedingDTOs)
                    {
                        var csvLine = $"{feeding.accountID},{feeding.userFeedID},{feeding.userName},{feeding.userFeedDate},{feeding.userFeedTime},{feeding.dailyMeal ?? "N/A"}," +
                                      $"{feeding.totalCarbohydrates},{feeding.totalProtein},{feeding.totalLipids},{feeding.totalCalories},{feeding.satietyLevel},\"{feeding.emotionsLinked}\",{feeding.saucerPictureUrl ?? "N/A"}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }

                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllCaloriesConsumedAsync(CancellationToken cancellationToken) 
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("CaloriesConsumedID,AccountID,Date,Total");

                while (currentPage >= 0)
                {
                    var calConsumed = await _bd.CaloriesConsumed
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    if (calConsumed.Count == 0)
                    {
                        break;
                    }

                    foreach (var cal in calConsumed)
                    {
                        var csvLine = $"{cal.caloriesConsumedID},{cal.accountID},{cal.dateCaloriesConsumed},{cal.totalCaloriesConsumed}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllCaloriesRequiredPerDaysAsync(CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("CaloriesPerDayID,AccountID,DateInitial,DateFinal,CaloriesNeeded");

                while (currentPage >= 0)
                {
                    var calConsumed = await _bd.CaloriesRequiredPerDays
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    if (calConsumed.Count == 0)
                    {
                        break;
                    }

                    foreach (var cal in calConsumed)
                    {
                        var csvLine = $"{cal.caloriesPerDayID},{cal.accountID},{cal.dateInitial},{cal.dateFinal},{cal.caloriesNeeded}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllUserCaloriesAsync(CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("UserCaloriesID,AccountID,CaloriesNeeded");

                while (currentPage >= 0)
                {
                    var calNeeded = await _bd.UserCalories
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    if (calNeeded.Count == 0)
                    {
                        break;
                    }

                    foreach (var cal in calNeeded)
                    {
                        var csvLine = $"{cal.userCaloriesID},{cal.accountID},{cal.caloriesNeeded}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllMFUsFeedingAsync(CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("MonthlyFollowUpID,AccountID,Month,Year,AnswQ1,AnswQ2,AnswQ3,AnswQ4,AnswQ5,AnswQ6,AnswQ7,AnswQ8,AnswQ9,TotalPts,Classification");

                while (currentPage >= 0)
                {
                    var mfus = await _bd.ResultsFood
                            .Include(m => m.MFUsFood)
                            .ThenInclude(m => m.months)
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    if (mfus.Count == 0)
                    {
                        break;
                    }

                    foreach (var m in mfus)
                    {
                        var csvLine = $"{m.monthlyFollowUpID},{m.MFUsFood.accountID},{m.MFUsFood.months.month},{m.MFUsFood.months.year},{m.MFUsFood.answerQuestion1}," +
                                      $"{m.MFUsFood.answerQuestion2},{m.MFUsFood.answerQuestion3},{m.MFUsFood.answerQuestion4},{m.MFUsFood.answerQuestion5},{m.MFUsFood.answerQuestion6},{m.MFUsFood.answerQuestion7},{m.MFUsFood.answerQuestion8},{m.MFUsFood.answerQuestion9}," +
                                      $"{m.totalPts},{m.classification}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }
    }
}
