using AppVidaSana.Data;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;
using AppVidaSana.Models.Feeding;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWFeedingService : IAWFeeding
    {
        private readonly AppDbContext _bd;

        public AWFeedingService(AppDbContext bd)
        {
            _bd = bd;
        }

        public async Task<List<AllFeedsOfAUserDto>> GetAllFeedsOfAUserAsync(UserFeedFilterDto filter, int page, CancellationToken cancellationToken)
        {
            var feedings = await GetQueryFeedings(filter, page, cancellationToken);

            var allFeedsOfAUser = feedings.Select(feeding => new AllFeedsOfAUserDto
            {
                accountID = feeding.account!.accountID,
                userFeedID = feeding.userFeedID,
                userName = feeding.account!.username,
                userFeedDate = feeding.userFeedDate,
                userFeedTime = feeding.userFeedTime,
                dailyMeal = feeding.dailyMeals?.dailyMeal ?? "N/A",
                totalCarbohydrates = feeding.userFeedNutritionalValues
                                             .Sum(nv => nv.nutritionalValues?.carbohydrates ?? 0 * nv.MealFrequency),
                totalProtein = feeding.userFeedNutritionalValues
                                             .Sum(nv => nv.nutritionalValues?.protein ?? 0 * nv.MealFrequency),
                totalLipids = feeding.userFeedNutritionalValues
                                             .Sum(nv => nv.nutritionalValues?.totalLipids ?? 0 * nv.MealFrequency),
                totalCalories = feeding.totalCalories,
                totalNetWeight = (double) feeding.userFeedNutritionalValues
                                             .Sum(nv => nv.nutritionalValues?.netWeight ?? 0 * nv.MealFrequency),
                satietyLevel = feeding.satietyLevel,
                emotionsLinked = feeding.emotionsLinked,
                saucerPictureUrl = feeding.saucerPicture?.saucerPictureUrl ?? "N/A"
            }).ToList();

            return allFeedsOfAUser;
        }

        public async Task<List<AllFoodsConsumedPerUserFeedDto>> GetAllFoodsConsumedPerUserFeedAsync(UserFeedFilterDto filter, int page, CancellationToken cancellationToken) 
        {
            var feedings = await GetQueryFeedings(filter, page, cancellationToken);

            var foodsConsumedPerUserFeeds = feedings
                .SelectMany(feeding => feeding.userFeedNutritionalValues
                    .SelectMany(nv => Enumerable.Range(0, nv.MealFrequency).Select(_ => new AllFoodsConsumedPerUserFeedDto
                        {
                            accountID = feeding.accountID,
                            userFeedID = feeding.userFeedID,
                            userFeedDate = feeding.userFeedDate,
                            userFeedTime = feeding.userFeedTime,
                            dailyMeal = feeding.dailyMeals?.dailyMeal ?? "N/A",

                            foodCode = nv.nutritionalValues?.foods?.foodCode ?? "",
                            nameFood = nv.nutritionalValues?.foods?.nameFood ?? "",
                            unit = nv.nutritionalValues?.foods?.unit ?? "",

                            nutritionalValueCode = nv.nutritionalValues?.nutritionalValueID.ToString() ?? "",
                            portion = nv.nutritionalValues?.portion ?? "",
                            kilocalories = nv.nutritionalValues?.kilocalories ?? 0,
                            protein = nv.nutritionalValues?.protein ?? 0,
                            carbohydrates = nv.nutritionalValues?.carbohydrates ?? 0,
                            totalLipids = nv.nutritionalValues?.totalLipids ?? 0,
                            netWeight = nv.nutritionalValues?.netWeight ?? 0
                        }))
                    )
                    .ToList();


            return foodsConsumedPerUserFeeds;
        }

        public async Task<byte[]> ExportAllFoodsConsumedPerFeedingAsync(CancellationToken cancellationToken) 
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("AccountID,UserFeedID,UserFeedDate,UserFeedTime,DailyMeal,FoodCode,NameFood,Unit,Portion,Carbohydrates,Protein,TotalLipids,Kcal,NetWeight");

                while (currentPage >= 0)
                {
                    var feedings = await _bd.UserFeeds
                            .Include(f => f.dailyMeals)
                            .Include(f => f.saucerPicture)
                            .Include(f => f.userFeedNutritionalValues)
                            .ThenInclude(nv => nv.nutritionalValues)
                            .ThenInclude(nv => nv!.foods)
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    var feedingDTOs = feedings
                        .SelectMany(feeding => feeding.userFeedNutritionalValues
                            .SelectMany(nv => Enumerable.Range(0, nv.MealFrequency).Select(_ => new AllFoodsConsumedPerUserFeedDto
                            {
                                accountID = feeding.accountID,
                                userFeedID = feeding.userFeedID,
                                userFeedDate = feeding.userFeedDate,
                                userFeedTime = feeding.userFeedTime,
                                dailyMeal = feeding.dailyMeals?.dailyMeal ?? "N/A",

                                foodCode = nv.nutritionalValues?.foods?.foodCode ?? "",
                                nameFood = nv.nutritionalValues?.foods?.nameFood ?? "",
                                unit = nv.nutritionalValues?.foods?.unit ?? "",

                                nutritionalValueCode = nv.nutritionalValues?.nutritionalValueID.ToString() ?? "",
                                portion = nv.nutritionalValues?.portion ?? "",
                                kilocalories = nv.nutritionalValues?.kilocalories ?? 0,
                                protein = nv.nutritionalValues?.protein ?? 0,
                                carbohydrates = nv.nutritionalValues?.carbohydrates ?? 0,
                                totalLipids = nv.nutritionalValues?.totalLipids ?? 0,
                                netWeight = nv.nutritionalValues?.netWeight ?? 0
                            }))
                            )
                            .ToList();

                    if (feedingDTOs.Count == 0)
                    {
                        currentPage = -1;
                    }
                    else 
                    { 
                        foreach (var feeding in feedingDTOs)
                        {
                            var csvLine = $"{feeding.accountID},{feeding.userFeedID},{feeding.userFeedDate},{feeding.userFeedTime},{feeding.dailyMeal ?? "N/A"}," +
                                        $"{feeding.foodCode},\"{feeding.nameFood}\",{feeding.unit},' {feeding.portion}',{Math.Round(feeding.carbohydrates, 2)},{Math.Round(feeding.protein, 2)},{Math.Round(feeding.totalLipids, 2)},{Math.Round(feeding.kilocalories, 2)},{feeding.netWeight}";

                            await streamWriter.WriteLineAsync(csvLine);
                        }
                        currentPage++;
                    }
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
                await streamWriter.WriteLineAsync("AccountID,UserFeedID,Username,UserFeedDate,UserFeedTime,DailyMeal,TotalCarbohydrates,TotalProtein,TotalLipids,TotalCalories,TotalNetWeight,SatietyLevel,EmotionsLinked,SaucerPictureUrl");

                while (currentPage >= 0)
                {
                    var feedings = await _bd.UserFeeds
                            .Include(f => f.account)
                            .Include(f => f.dailyMeals)
                            .Include(f => f.saucerPicture)
                            .Include(f => f.userFeedNutritionalValues)
                            .ThenInclude(nv => nv.nutritionalValues)
                            .ThenInclude(nv => nv!.foods)
                            .Where(f => f.dailyMeals.dailyMeal == "Desayuno")
                            .OrderBy(f => f.userFeedID)
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    var feedingDTOs = feedings.Select(feeding => new AllFeedsOfAUserDto
                    {
                        accountID = feeding.account!.accountID,
                        userFeedID = feeding.userFeedID, 
                        userName = feeding.account!.username,
                        userFeedDate = feeding.userFeedDate,
                        userFeedTime = feeding.userFeedTime,
                        dailyMeal = feeding.dailyMeals?.dailyMeal ?? "N/A",
                        totalCarbohydrates = feeding.userFeedNutritionalValues
                                             .Sum(nv => nv.nutritionalValues?.carbohydrates ?? 0 * nv.MealFrequency),
                        totalProtein = feeding.userFeedNutritionalValues
                                             .Sum(nv => nv.nutritionalValues?.protein ?? 0 * nv.MealFrequency),
                        totalLipids = feeding.userFeedNutritionalValues
                                             .Sum(nv => nv.nutritionalValues?.totalLipids ?? 0 * nv.MealFrequency),
                        totalCalories = feeding.totalCalories,
                        totalNetWeight = (double) feeding.userFeedNutritionalValues
                                                    .Sum(nv => nv.nutritionalValues?.netWeight ?? 0 * nv.MealFrequency),
                        satietyLevel = feeding.satietyLevel,
                        emotionsLinked = feeding.emotionsLinked,
                        saucerPictureUrl = feeding.saucerPicture?.saucerPictureUrl
                    }).ToList();

                    if (feedingDTOs.Count == 0)
                    {
                        currentPage = -1;
                    }
                    else 
                    { 
                        foreach (var feeding in feedingDTOs)
                        {
                            var csvLine = $"{feeding.accountID},{feeding.userFeedID},{feeding.userName},{feeding.userFeedDate},{feeding.userFeedTime},{feeding.dailyMeal ?? "N/A"}," +
                                          $"{Math.Round(feeding.totalCarbohydrates, 2)},{Math.Round(feeding.totalProtein, 2)},{Math.Round(feeding.totalLipids, 2)},{Math.Round(feeding.totalCalories, 2)},{Math.Round(feeding.totalNetWeight, 2)},{feeding.satietyLevel},\"{feeding.emotionsLinked}\",{feeding.saucerPictureUrl ?? "N/A"}";

                            await streamWriter.WriteLineAsync(csvLine);
                        }

                        currentPage++;
                    }
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
                        currentPage = -1;
                    }
                    else 
                    { 
                        foreach (var cal in calConsumed)
                        {
                            var csvLine = $"{cal.caloriesConsumedID},{cal.accountID},{cal.dateCaloriesConsumed},{cal.totalCaloriesConsumed}";

                            await streamWriter.WriteLineAsync(csvLine);
                        }

                        currentPage++;
                    }
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
                        currentPage = -1;
                    }
                    else 
                    { 
                        foreach (var cal in calConsumed)
                        {
                            var csvLine = $"{cal.caloriesPerDayID},{cal.accountID},{cal.dateInitial},{cal.dateFinal},{cal.caloriesNeeded}";

                            await streamWriter.WriteLineAsync(csvLine);
                        }

                        currentPage++;
                    }
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
                        currentPage = -1;
                    }
                    else 
                    {
                        foreach (var cal in calNeeded)
                        {
                            var csvLine = $"{cal.userCaloriesID},{cal.accountID},{cal.caloriesNeeded}";

                            await streamWriter.WriteLineAsync(csvLine);
                        }

                        currentPage++;
                    }
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
                            .ThenInclude(m => m!.months)
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    if (mfus.Count == 0)
                    {
                        currentPage = -1;
                    }
                    else 
                    { 
                        foreach (var m in mfus)
                        {
                            var csvLine = $"{m.monthlyFollowUpID},{m.MFUsFood!.accountID},{m.MFUsFood!.months!.month},{m.MFUsFood.months.year},{m.MFUsFood.answerQuestion1}," +
                                          $"{m.MFUsFood.answerQuestion2},{m.MFUsFood.answerQuestion3},{m.MFUsFood.answerQuestion4},{m.MFUsFood.answerQuestion5},{m.MFUsFood.answerQuestion6},{m.MFUsFood.answerQuestion7},{m.MFUsFood.answerQuestion8},{m.MFUsFood.answerQuestion9}," +
                                          $"{m.totalPts},{m.classification}";

                            await streamWriter.WriteLineAsync(csvLine);
                        }
                        currentPage++;
                    }
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        private async Task<List<UserFeeds>> GetQueryFeedings(UserFeedFilterDto filter, int page, CancellationToken cancellationToken) {

            var query = _bd.UserFeeds
                        .Include(f => f.account)
                        .Include(f => f.dailyMeals)
                        .Include(f => f.saucerPicture)
                        .Include(f => f.userFeedNutritionalValues)
                            .ThenInclude(nv => nv.nutritionalValues)
                                .ThenInclude(nv => nv!.foods)
                        .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.accountID.ToString()))
                query = query.Where(f => f.account!.accountID.ToString().Contains(filter.accountID.ToString() ?? ""));

            if (!string.IsNullOrWhiteSpace(filter.username))
                query = query.Where(f => f.account!.username.Contains(filter.username ?? ""));

            if (!string.IsNullOrWhiteSpace(filter.uiemID))
                query = query.Where(f => _bd.Profiles
                             .Any(p => p.accountID == f.account!.accountID && p.uiemID == filter.uiemID));

            if (!string.IsNullOrWhiteSpace(filter.startDate.ToString()))
                query = query.Where(f => f.userFeedDate >= filter.startDate);

            if (!string.IsNullOrWhiteSpace(filter.endDate.ToString()))
                query = query.Where(f => f.userFeedDate <= filter.endDate);

            if (!string.IsNullOrWhiteSpace(filter.month.ToString()))
                query = query.Where(f => _bd.Profiles
                             .Any(p => p.accountID == f.account!.accountID && p.birthDate.Month == filter.month));

            if (!string.IsNullOrWhiteSpace(filter.year.ToString()))
                query = query.Where(f => _bd.Profiles
                             .Any(p => p.accountID == f.account!.accountID && p.birthDate.Year == filter.year));

            if (!string.IsNullOrWhiteSpace(filter.sex))
                query = query.Where(f => _bd.Profiles
                             .Any(p => p.accountID == f.account!.accountID && p.sex == filter.sex));

            if (!string.IsNullOrWhiteSpace(filter.protocolToFollow))
                query = query.Where(f => _bd.Profiles
                             .Any(p => p.accountID == f.account!.accountID && p.protocolToFollow == filter.protocolToFollow));

            if (!string.IsNullOrWhiteSpace(filter.dailyMeal))
                query = query.Where(f => f.dailyMeals!.dailyMeal.Contains(filter.dailyMeal ?? ""));

            var feedings = await query
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync(cancellationToken);

            return feedings;
        }
    }
}
