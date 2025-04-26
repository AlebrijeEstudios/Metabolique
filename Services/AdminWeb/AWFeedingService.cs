using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;
using AppVidaSana.Models.Feeding;
using AppVidaSana.Models.Monthly_Follow_Ups.Results;
using AppVidaSana.Months_Dates;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWFeedingService : IAWFeeding
    {
        private readonly AppDbContext _bd;
        private readonly IHttpContextAccessor _httpContextAccessor;


        public AWFeedingService(AppDbContext bd, IHttpContextAccessor httpContextAccessor)
        {
            _bd = bd;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<AllFeedsOfAUserDto>> GetAllFeedsOfAUserAsync(UserFeedFilterDto filter, int page, CancellationToken cancellationToken)
        {
            var role = UserRole();

            if (role == "Admin") 
            {
                var feedings = await GetQueryFeedingsAsync(filter, page, false, 0, cancellationToken);

                var allFeedsOfAUser = feedings.Select(feeding => new AllFeedsOfAUserDto
                {
                    accountID = feeding.account!.accountID,
                    userFeedID = feeding.userFeedID,
                    username = feeding.account!.username,
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
                    totalNetWeight = (double)feeding.userFeedNutritionalValues
                                                 .Sum(nv => nv.nutritionalValues?.netWeight ?? 0 * nv.MealFrequency),
                    satietyLevel = feeding.satietyLevel,
                    emotionsLinked = feeding.emotionsLinked,
                    saucerPictureUrl = feeding.saucerPicture?.saucerPictureUrl ?? "N/A"
                }).ToList();

                return allFeedsOfAUser;
            }

            return [];
        }

        public async Task<List<AllFoodsConsumedPerUserFeedDto>> GetAllFoodsConsumedPerUserFeedAsync(UserFeedFilterDto filter, int page, CancellationToken cancellationToken) 
        {
            var role = UserRole();

            if (role == "Admin") 
            {
                var feedings = await GetQueryFeedingsAsync(filter, page, false, 0, cancellationToken);

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

            return [];
        }

        public async Task<List<AllCaloriesConsumedPerUserDto>> GetAllCaloriesConsumedPerUserAsync(CaloriesConsumedFilterDto filter, int page, CancellationToken cancellationToken) 
        {
            var role = UserRole();

            if (role == "Admin") 
            { 
                var kcalConsumed = await GetQueryKcalConsumedAsync(filter, page, false, 0, cancellationToken);

                var allKcalConsumedPerUser = kcalConsumed.Select(kcal => new AllCaloriesConsumedPerUserDto
                {
                    caloriesConsumedID = kcal.caloriesConsumedID,
                    accountID = kcal.accountID,
                    username = kcal.account?.username ?? "N/A",
                    date = kcal.dateCaloriesConsumed,
                    totalKcal = kcal.totalCaloriesConsumed
                }).ToList();

                return allKcalConsumedPerUser;
            }

            return [];
        }

        public async Task<List<AllCaloriesRequiredPerDaysDto>> GetAllCaloriesRequiredPerDaysAsync(CaloriesRequiredPerDaysFilterDto filter, int page, CancellationToken cancellationToken) 
        {
            var role = UserRole();

            if (role == "Admin") 
            {
                var kcalReq = await GetQueryKcalRequiredPerDaysAsync(filter, page, false, 0, cancellationToken);

                var allKcalRequiredPerDay = kcalReq.Select(kcal => new AllCaloriesRequiredPerDaysDto
                {
                    caloriesPerDayID = kcal.caloriesPerDayID,
                    accountID = kcal.accountID,
                    username = kcal.account?.username ?? "N/A",
                    dateInitial = kcal.dateInitial,
                    dateFinal = kcal.dateFinal,
                    kcalNeeded = kcal.caloriesNeeded
                }).ToList();

                return allKcalRequiredPerDay;
            }

            return [];
        }

        public async Task<List<AllUserCaloriesDto>> GetAllUserCaloriesAsync(PatientFilterDto filter, int page, CancellationToken cancellationToken) 
        {
            var role = UserRole();

            if (role == "Admin") 
            {
                var kcalUsers = await GetQueryUserCaloriesAsync(filter, page, false, 0, cancellationToken);

                var allKcalRequiredPerUser = kcalUsers.Select(kcal => new AllUserCaloriesDto
                {
                    userCaloriesID = kcal.userCaloriesID,
                    accountID = kcal.accountID,
                    username = kcal.account?.username ?? "N/A",
                    kcalNeeded = kcal.caloriesNeeded
                }).ToList();

                return allKcalRequiredPerUser;
            }

            return [];
        }

        public async Task<List<AllMFUsFeedingPerUserDto>> GetMFUsFeedingAsync(PatientFilterDto filter, int page, CancellationToken cancellationToken) 
        {
            var role = UserRole();

            if (role == "Admin") 
            {
                var mfus = await GetQueryMFUsFeedingAsync(filter, page, false, 0, cancellationToken);

                var allMFUsPerUser = mfus.Select(m => new AllMFUsFeedingPerUserDto
                {
                    monthlyFollowUpID = m.monthlyFollowUpID,
                    accountID = m.MFUsFood!.accountID,
                    username = m.MFUsFood!.account!.username,
                    month = m.MFUsFood!.months!.month,
                    year = m.MFUsFood!.months!.year,
                    answerQuestion1 = m.MFUsFood!.answerQuestion1,
                    answerQuestion2 = m.MFUsFood!.answerQuestion2,
                    answerQuestion3 = m.MFUsFood!.answerQuestion3,
                    answerQuestion4 = m.MFUsFood!.answerQuestion4,
                    answerQuestion5 = m.MFUsFood!.answerQuestion5,
                    answerQuestion6 = m.MFUsFood!.answerQuestion6,
                    answerQuestion7 = m.MFUsFood!.answerQuestion7,
                    answerQuestion8 = m.MFUsFood!.answerQuestion8,
                    answerQuestion9 = m.MFUsFood!.answerQuestion9,
                    totalPts = m.totalPts,
                    classification = m.classification
                }).ToList();

                return allMFUsPerUser;
            }

            return [];
        }


        public async Task<byte[]> ExportAllFeedingsAsync(UserFeedFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("AccountID,UserFeedID,Username,UserFeedDate,UserFeedTime,DailyMeal,TotalCarbohydrates,TotalProtein,TotalLipids,TotalCalories,TotalNetWeight,SatietyLevel,EmotionsLinked,SaucerPictureUrl");

                while (currentPage >= 0)
                {
                    var feedings = await GetQueryFeedingsAsync(filter, 0, true, currentPage, cancellationToken);

                    var feedingDTOs = feedings.Select(feeding => new AllFeedsOfAUserDto
                    {
                        accountID = feeding.account!.accountID,
                        userFeedID = feeding.userFeedID,
                        username = feeding.account!.username,
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
                        totalNetWeight = (double)feeding.userFeedNutritionalValues
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
                            var csvLine = $"{feeding.accountID},{feeding.userFeedID},{feeding.username},{feeding.userFeedDate},{feeding.userFeedTime},{feeding.dailyMeal ?? "N/A"}," +
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

        public async Task<byte[]> ExportAllFoodsConsumedPerFeedingAsync(UserFeedFilterDto? filter, CancellationToken cancellationToken) 
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("AccountID,UserFeedID,UserFeedDate,UserFeedTime,DailyMeal,FoodCode,NameFood,Unit,Portion,Carbohydrates,Protein,TotalLipids,Kcal,NetWeight");

                while (currentPage >= 0)
                {
                    var feedings = await GetQueryFeedingsAsync(filter, 0, true, currentPage, cancellationToken);

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

        public async Task<byte[]> ExportAllCaloriesConsumedAsync(CaloriesConsumedFilterDto? filter, CancellationToken cancellationToken) 
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("CaloriesConsumedID,AccountID,Username,Date,Total");

                while (currentPage >= 0)
                {
                    var calConsumed = await GetQueryKcalConsumedAsync(filter, 0, true, currentPage, cancellationToken);

                    if (calConsumed.Count == 0)
                    {
                        currentPage = -1;
                    }
                    else 
                    { 
                        foreach (var cal in calConsumed)
                        {
                            var csvLine = $"{cal.caloriesConsumedID},{cal.accountID},{cal.account?.username ?? "N/A"},{cal.dateCaloriesConsumed},{cal.totalCaloriesConsumed}";

                            await streamWriter.WriteLineAsync(csvLine);
                        }

                        currentPage++;
                    }
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllCaloriesRequiredPerDaysAsync(CaloriesRequiredPerDaysFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("CaloriesPerDayID,AccountID,Username,DateInitial,DateFinal,CaloriesNeeded");

                while (currentPage >= 0)
                {
                    var calReq = await GetQueryKcalRequiredPerDaysAsync(filter, 0, true, currentPage, cancellationToken);

                    if (calReq.Count == 0)
                    {
                        currentPage = -1;
                    }
                    else 
                    { 
                        foreach (var cal in calReq)
                        {
                            var csvLine = $"{cal.caloriesPerDayID},{cal.accountID},{cal.account?.username ?? "N/A"},{cal.dateInitial},{cal.dateFinal},{cal.caloriesNeeded}";

                            await streamWriter.WriteLineAsync(csvLine);
                        }

                        currentPage++;
                    }
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllUserCaloriesAsync(PatientFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("UserCaloriesID,AccountID,Username,CaloriesNeeded");

                while (currentPage >= 0)
                {
                    var calNeeded = await GetQueryUserCaloriesAsync(filter, 0, true, currentPage, cancellationToken);

                    if (calNeeded.Count == 0)
                    {
                        currentPage = -1;
                    }
                    else 
                    {
                        foreach (var cal in calNeeded)
                        {
                            var csvLine = $"{cal.userCaloriesID},{cal.accountID},{cal.account?.username ?? "N/A"},{cal.caloriesNeeded}";

                            await streamWriter.WriteLineAsync(csvLine);
                        }

                        currentPage++;
                    }
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllMFUsFeedingAsync(PatientFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("MonthlyFollowUpID,AccountID,Username,Month,Year,AnswQ1,AnswQ2,AnswQ3,AnswQ4,AnswQ5,AnswQ6,AnswQ7,AnswQ8,AnswQ9,TotalPts,Classification");

                while (currentPage >= 0)
                {
                    var mfus = await GetQueryMFUsFeedingAsync(filter, 0, true, currentPage, cancellationToken);

                    if (mfus.Count == 0)
                    {
                        currentPage = -1;
                    }
                    else 
                    { 
                        foreach (var m in mfus)
                        {
                            var csvLine = $"{m.monthlyFollowUpID},{m.MFUsFood!.accountID},{m.MFUsFood!.account!.username},{m.MFUsFood!.months!.month},{m.MFUsFood.months.year},{m.MFUsFood.answerQuestion1}," +
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


        private string UserRole() 
        {
            var role = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (role is null) { throw new UnstoredValuesException(); }

            return role;
        }

        private async Task<List<UserFeeds>> GetQueryFeedingsAsync(UserFeedFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken) 
        {
            List<UserFeeds> feedings = new List<UserFeeds>();

            var query = _bd.UserFeeds
                        .Include(f => f.account)
                        .Include(f => f.dailyMeals)
                        .Include(f => f.saucerPicture)
                        .Include(f => f.userFeedNutritionalValues)
                            .ThenInclude(nv => nv.nutritionalValues)
                                .ThenInclude(nv => nv!.foods)
                        .AsQueryable();

            if (filter != null) 
            {
                query = query.Where(p => _bd.PacientDoctor
                                        .Where(pd => pd.doctorID == filter.doctorID)
                                        .Select(pd => pd.accountID)
                                        .Contains(p.account!.accountID));

                if (!string.IsNullOrWhiteSpace(filter.accountID.ToString()))
                    query = query.Where(f => f.account!.accountID.ToString().Contains(filter.accountID.ToString() ?? ""));

                if (!string.IsNullOrWhiteSpace(filter.username))
                    query = query.Where(f => f.account!.username.Contains(filter.username ?? ""));

                if (!string.IsNullOrWhiteSpace(filter.uiemID))
                    query = query.Where(f => _bd.Profiles
                                 .Any(p => p.accountID == f.account!.accountID && p.uiemID == filter.uiemID));

                if (filter.startDate != null && filter.endDate != null)
                {
                    query = query.Where(f =>
                        f.userFeedDate <= filter.endDate &&
                        f.userFeedDate >= filter.startDate
                    );
                }
                else if (filter.startDate != null)
                {
                    query = query.Where(f =>
                        f.userFeedDate >= filter.startDate
                    );
                }
                else if (filter.endDate != null)
                {
                    query = query.Where(f =>
                        f.userFeedDate <= filter.endDate
                    );
                }

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
            }

            if (!export)
            {
                feedings = await query
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync(cancellationToken);
            }
            else
            {
                feedings = await query
                            .Skip(currentPage * 1000)
                            .Take(1000)
                            .ToListAsync(cancellationToken);
            }

            return feedings;
        }

        private async Task<List<CaloriesConsumed>> GetQueryKcalConsumedAsync(CaloriesConsumedFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken) 
        {
            List<CaloriesConsumed> kcalConsumed = new List<CaloriesConsumed>();

            var query = _bd.CaloriesConsumed
                            .Include(f => f.account)
                            .AsQueryable();

            if (filter != null) 
            {
                query = query.Where(p => _bd.PacientDoctor
                                          .Where(pd => pd.doctorID == filter.doctorID)
                                          .Select(pd => pd.accountID)
                                          .Contains(p.account!.accountID));

                if (!string.IsNullOrWhiteSpace(filter.accountID.ToString()))
                    query = query.Where(f => f.account!.accountID.ToString().Contains(filter.accountID.ToString() ?? ""));

                if (!string.IsNullOrWhiteSpace(filter.username))
                    query = query.Where(f => f.account!.username.Contains(filter.username ?? ""));

                if (!string.IsNullOrWhiteSpace(filter.uiemID))
                    query = query.Where(f => _bd.Profiles
                                 .Any(p => p.accountID == f.account!.accountID && p.uiemID == filter.uiemID));

                if (filter.startDate != null && filter.endDate != null)
                {
                    query = query.Where(f =>
                        f.dateCaloriesConsumed <= filter.endDate &&
                        f.dateCaloriesConsumed >= filter.startDate
                    );
                }
                else if (filter.startDate != null)
                {
                    query = query.Where(f =>
                        f.dateCaloriesConsumed >= filter.startDate
                    );
                }
                else if (filter.endDate != null)
                {
                    query = query.Where(f =>
                        f.dateCaloriesConsumed <= filter.endDate
                    );
                }

                if (!string.IsNullOrWhiteSpace(filter.sex))
                    query = query.Where(f => _bd.Profiles
                                 .Any(p => p.accountID == f.account!.accountID && p.sex == filter.sex));

                if (!string.IsNullOrWhiteSpace(filter.protocolToFollow))
                    query = query.Where(f => _bd.Profiles
                                 .Any(p => p.accountID == f.account!.accountID && p.protocolToFollow == filter.protocolToFollow));
            }

            if (!export)
            {
                kcalConsumed = await query
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync(cancellationToken);
            }
            else
            {
                kcalConsumed = await query
                            .Skip(currentPage * 1000)
                            .Take(1000)
                            .ToListAsync(cancellationToken);
            }

            return kcalConsumed;
        }

        private async Task<List<CaloriesRequiredPerDay>> GetQueryKcalRequiredPerDaysAsync(CaloriesRequiredPerDaysFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken) 
        {
            List<CaloriesRequiredPerDay> kcalReqPerDays = new List<CaloriesRequiredPerDay>();

            var query = _bd.CaloriesRequiredPerDays
                            .Include(f => f.account)
                            .AsQueryable();

            if (filter != null) 
            {
                query = query.Where(p => _bd.PacientDoctor
                                          .Where(pd => pd.doctorID == filter.doctorID)
                                          .Select(pd => pd.accountID)
                                          .Contains(p.account!.accountID));

                if (!string.IsNullOrWhiteSpace(filter.accountID.ToString()))
                    query = query.Where(f => f.account!.accountID.ToString().Contains(filter.accountID.ToString() ?? ""));

                if (!string.IsNullOrWhiteSpace(filter.username))
                    query = query.Where(f => f.account!.username.Contains(filter.username ?? ""));

                if (!string.IsNullOrWhiteSpace(filter.uiemID))
                    query = query.Where(f => _bd.Profiles
                                 .Any(p => p.accountID == f.account!.accountID && p.uiemID == filter.uiemID));

                if (filter.startDate != null && filter.endDate != null)
                {
                    query = query.Where(f =>
                        f.dateInitial <= filter.endDate &&
                        f.dateFinal >= filter.startDate
                    );
                }
                else if (filter.startDate != null)
                {
                    query = query.Where(f =>
                        f.dateFinal >= filter.startDate
                    );
                }
                else if (filter.endDate != null)
                {
                    query = query.Where(f =>
                        f.dateInitial <= filter.endDate
                    );
                }

                if (!string.IsNullOrWhiteSpace(filter.sex))
                    query = query.Where(f => _bd.Profiles
                                 .Any(p => p.accountID == f.account!.accountID && p.sex == filter.sex));

                if (!string.IsNullOrWhiteSpace(filter.protocolToFollow))
                    query = query.Where(f => _bd.Profiles
                                 .Any(p => p.accountID == f.account!.accountID && p.protocolToFollow == filter.protocolToFollow));
            }

            if (!export)
            {
                kcalReqPerDays = await query
                                .Skip((page - 1) * 10)
                                .Take(10)
                                .ToListAsync(cancellationToken);
            }
            else
            {
                kcalReqPerDays = await query
                                .Skip(currentPage * 1000)
                                .Take(1000)
                                .ToListAsync(cancellationToken);
            }

            return kcalReqPerDays;
        }

        private async Task<List<UserCalories>> GetQueryUserCaloriesAsync(PatientFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken) 
        {
            List<UserCalories> kcalNeeded = new List<UserCalories>();

            var query = _bd.UserCalories
                            .Include(f => f.account)
                            .AsQueryable();

            if (filter != null) 
            {
                query = query.Where(p => _bd.PacientDoctor
                                          .Where(pd => pd.doctorID == filter.doctorID)
                                          .Select(pd => pd.accountID)
                                          .Contains(p.account!.accountID));

                if (!string.IsNullOrWhiteSpace(filter.accountID.ToString()))
                    query = query.Where(f => f.account!.accountID.ToString().Contains(filter.accountID.ToString() ?? ""));

                if (!string.IsNullOrWhiteSpace(filter.username))
                    query = query.Where(f => f.account!.username.Contains(filter.username ?? ""));

                if (!string.IsNullOrWhiteSpace(filter.uiemID))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.account!.accountID && p.uiemID == filter.uiemID));

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
            }

            if (!export)
            {
                kcalNeeded = await query
                                .Skip((page - 1) * 10)
                                .Take(10)
                                .ToListAsync(cancellationToken);
            }
            else
            {
                kcalNeeded = await query
                                .Skip(currentPage * 1000)
                                .Take(1000)
                                .ToListAsync(cancellationToken);
            }

            return kcalNeeded;
        }

        private async Task<List<FoodResults>> GetQueryMFUsFeedingAsync(PatientFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken) 
        {
            List<FoodResults> mfu = new List<FoodResults>();

            var query = _bd.ResultsFood
                        .Include(rf => rf.MFUsFood)                       
                            .ThenInclude(mf => mf!.account)                
                        .Include(rf => rf.MFUsFood)                      
                            .ThenInclude(mf => mf!.months)                
                        .AsQueryable();

            if (filter != null) 
            {
                var monthStr = Months.VerifyExistMonth(filter?.month ?? 0);

                query = query.Where(p => _bd.PacientDoctor
                                          .Where(pd => pd.doctorID == filter!.doctorID)
                                          .Select(pd => pd.accountID)
                                          .Contains(p.MFUsFood!.account!.accountID));

                if (!string.IsNullOrWhiteSpace(filter!.accountID.ToString()))
                    query = query.Where(f => f.MFUsFood!.account!.accountID.ToString().Contains(filter.accountID.ToString() ?? ""));

                if (!string.IsNullOrWhiteSpace(filter!.username))
                    query = query.Where(f => f.MFUsFood!.account!.username.Contains(filter.username ?? ""));

                if (!string.IsNullOrWhiteSpace(filter!.uiemID))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.MFUsFood!.account!.accountID && p.uiemID == filter.uiemID));

                if (!string.IsNullOrWhiteSpace(filter!.month.ToString()))
                    query = query.Where(f => f.MFUsFood!.months!.month.Contains(monthStr));

                if (!string.IsNullOrWhiteSpace(filter!.year.ToString()))
                    query = query.Where(f => f.MFUsFood!.months!.year == filter.year);

                if (!string.IsNullOrWhiteSpace(filter!.sex))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.MFUsFood!.account!.accountID && p.sex == filter.sex));

                if (!string.IsNullOrWhiteSpace(filter!.protocolToFollow))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.MFUsFood!.account!.accountID && p.protocolToFollow == filter.protocolToFollow));
            }

            if (!export)
            {
                mfu = await query
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync(cancellationToken);
            }
            else
            {
                mfu = await query
                            .Skip(currentPage * 1000)
                            .Take(1000)
                            .ToListAsync(cancellationToken);
            }

            return mfu;
        }
    }
}
