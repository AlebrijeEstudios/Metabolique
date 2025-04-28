using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Habits_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
using AppVidaSana.Models.Habits;
using AppVidaSana.Models.Monthly_Follow_Ups.Results;
using AppVidaSana.Months_Dates;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWHabitService : IAWHabits
    {
        private readonly AppDbContext _bd;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AWHabitService(AppDbContext bd, IHttpContextAccessor httpContextAccessor)
        {
            _bd = bd;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<AllHabitDrinkPerUserDto>> GetAllHabitsDrinkPerUserAsync(HabitDrinkFilterDto filter, int page, CancellationToken cancellationToken) 
        {
            var role = UserRole();

            if (role == "Admin") 
            {
                var hDrink = await GetQueryHabitDrinkAsync(filter, page, false, 0, cancellationToken);

                var allHabitsDrinkPerUser = hDrink.Select(hD => new AllHabitDrinkPerUserDto
                {
                    drinkHabitID = hD.drinkHabitID,
                    accountID = hD.accountID,
                    username = hD.account!.username,
                    dateHabit = hD.drinkDateHabit,
                    amountConsumed = hD.amountConsumed ?? 0
                }).ToList();

                return allHabitsDrinkPerUser;
            }

            return [];
        }

        public async Task<List<AllHabitDrugPerUserDto>> GetAllHabitsDrugsPerUserAsync(HabitDrugFilterDto filter, int page, CancellationToken cancellationToken) 
        {
            var role = UserRole();

            if (role == "Admin") 
            {
                var hDrug = await GetQueryHabitDrugAsync(filter, page, false, 0, cancellationToken);

                var allHabitsDrugPerUser = hDrug.Select(hD => new AllHabitDrugPerUserDto
                {
                    drugsHabitID = hD.drugsHabitID,
                    accountID = hD.accountID,
                    username = hD.account!.username,
                    dateHabit = hD.drugsDateHabit,
                    cigarettesSmoked = hD.cigarettesSmoked ?? 0,
                    predominantEmotionalState = hD.predominantEmotionalState ?? "N/A"
                }).ToList();

                return allHabitsDrugPerUser;

            }

            return [];
        }

        public async Task<List<AllHabitSleepPerUserDto>> GetAllHabitsSleepPerUserAsync(HabitSleepFilterDto filter, int page, CancellationToken cancellationToken) 
        {
            var role = UserRole();

            if (role == "Admin") 
            {
                var hSleep = await GetQueryHabitSleepAsync(filter, page, false, 0, cancellationToken);

                var allHabitsSleepPerUser = hSleep.Select(hD => new AllHabitSleepPerUserDto
                {
                    sleepHabitID = hD.sleepHabitID,
                    accountID = hD.accountID,
                    username = hD.account!.username,
                    dateHabit = hD.sleepDateHabit,
                    sleepHours = hD.sleepHours ?? 0,
                    perceptionOfRelaxation = hD.perceptionOfRelaxation ?? "N/A"
                }).ToList();

                return allHabitsSleepPerUser;
            }

            return [];
        }

        public async Task<List<AllMFUsHabitsPerUserDto>> GetMFUsHabitsAsync(PatientFilterDto filter, int page, CancellationToken cancellationToken)
        {
            var role = UserRole();

            if (role == "Admin")
            {
                var mfus = await GetQueryMFUsHabitsAsync(filter, page, false, 0, cancellationToken);

                var allMFUsPerUser = mfus.Select(m => new AllMFUsHabitsPerUserDto
                {
                    monthlyFollowUpID = m.monthlyFollowUpID,
                    accountID = m.MFUsHabits!.accountID,
                    username = m.MFUsHabits!.account!.username,
                    month = m.MFUsHabits!.months!.month,
                    year = m.MFUsHabits!.months!.year,
                    answerQuestion1 = m.MFUsHabits!.answerQuestion1,
                    answerQuestion2 = m.MFUsHabits!.answerQuestion2,
                    answerQuestion3 = m.MFUsHabits!.answerQuestion3,
                    answerQuestion4 = m.MFUsHabits!.answerQuestion4,
                    answerQuestion5a = m.MFUsHabits!.answerQuestion5a,
                    answerQuestion5b = m.MFUsHabits!.answerQuestion5b,
                    answerQuestion5c = m.MFUsHabits!.answerQuestion5c,
                    answerQuestion5d = m.MFUsHabits!.answerQuestion5d,
                    answerQuestion5e = m.MFUsHabits!.answerQuestion5e,
                    answerQuestion5f = m.MFUsHabits!.answerQuestion5f,
                    answerQuestion5h = m.MFUsHabits!.answerQuestion5h,
                    answerQuestion5i = m.MFUsHabits!.answerQuestion5i,
                    answerQuestion5j = m.MFUsHabits!.answerQuestion5j,
                    answerQuestion6 = m.MFUsHabits!.answerQuestion6,
                    answerQuestion7 = m.MFUsHabits!.answerQuestion7,
                    answerQuestion8 = m.MFUsHabits!.answerQuestion8,
                    answerQuestion9 = m.MFUsHabits!.answerQuestion9,
                    resultComponent1 = m.resultComponent1,
                    resultComponent2 = m.resultComponent2,
                    resultComponent3 = m.resultComponent3,
                    resultComponent4 = m.resultComponent4,
                    resultComponent5 = m.resultComponent5,
                    resultComponent6 = m.resultComponent6,
                    resultComponent7 = m.resultComponent7,
                    globalClassification = m.globalClassification,
                    classification = m.classification
                }).ToList();

                return allMFUsPerUser;
            }

            return [];
        }


        public async Task<byte[]> ExportAllHabitsDrinkAsync(HabitDrinkFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("DrinkHabitID,AccountID,Username,DateHabit,AmountConsumed");

                while (currentPage >= 0)
                {
                    var hDrink = await GetQueryHabitDrinkAsync(filter, 0, true, currentPage, cancellationToken);

                    if (hDrink.Count == 0)
                    {
                        break;
                    }

                    foreach (var h in hDrink)
                    {
                        var csvLine = $"{h.drinkHabitID},{h.accountID},{h.account!.username},{h.drinkDateHabit},{h.amountConsumed}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllHabitsDrugsAsync(HabitDrugFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("DrugsHabitID,AccountID,Username,DateHabit,CigarettesSmoked,PredominantEmotionalState");

                while (currentPage >= 0)
                {
                    var hDrugs = await GetQueryHabitDrugAsync(filter, 0, true, currentPage, cancellationToken);

                    if (hDrugs.Count == 0)
                    {
                        break;
                    }

                    foreach (var h in hDrugs)
                    {
                        var csvLine = $"{h.drugsHabitID},{h.accountID},{h.account!.username},{h.drugsDateHabit},{h.cigarettesSmoked},{h.predominantEmotionalState}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllHabitsSleepAsync(HabitSleepFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("SleepHabitID,AccountID,Username,DateHabit,SleepHours,PerceptionOfRelaxation");

                while (currentPage >= 0)
                {
                    var hSleep = await GetQueryHabitSleepAsync(filter, 0, true, currentPage, cancellationToken);

                    if (hSleep.Count == 0)
                    {
                        break;
                    }

                    foreach (var h in hSleep)
                    {
                        var csvLine = $"{h.sleepHabitID},{h.accountID},{h.account!.username},{h.sleepDateHabit},{h.sleepHours},{h.perceptionOfRelaxation}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllMFUsHabitsAsync(PatientFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("MonthlyFollowUpID,AccountID,Username,Month,Year,AnswQ1,AnswQ2,AnswQ3,AnswQ4,AnswQ5a,AnswQ5b,AnswQ5c,AnswQ5d,AnswQ5e,AnswQ5f,AnswQ5g,AnswQ5h,AnswQ5i,AnswQ5j,AnswQ6,AnswQ7,AnswQ8,AnswQ9,ResultComponent_1,ResultComponent_2,ResultComponent_3,ResultComponent_4,ResultComponent_5,ResultComponent_6,ResultComponent_7,GlobalClassification,Classification");

                while (currentPage >= 0)
                {
                    var mfus = await GetQueryMFUsHabitsAsync(filter, 0, true, currentPage, cancellationToken);

                    if (mfus.Count == 0)
                    {
                        break;
                    }

                    foreach (var m in mfus)
                    {
                        var csvLine = $"{m.monthlyFollowUpID},{m.MFUsHabits!.accountID},{m.MFUsHabits!.account!.username},{m.MFUsHabits!.months!.month},{m.MFUsHabits!.months!.year},{m.MFUsHabits!.answerQuestion1}," +
                                      $"{m.MFUsHabits!.answerQuestion2},{m.MFUsHabits!.answerQuestion3},{m.MFUsHabits!.answerQuestion4},{m.MFUsHabits!.answerQuestion5a}," +
                                      $"{m.MFUsHabits!.answerQuestion5b},{m.MFUsHabits!.answerQuestion5c},{m.MFUsHabits!.answerQuestion5d},{m.MFUsHabits!.answerQuestion5e},{m.MFUsHabits!.answerQuestion5f}," +
                                      $"{m.MFUsHabits!.answerQuestion5g},{m.MFUsHabits!.answerQuestion5h},{m.MFUsHabits!.answerQuestion5i},{m.MFUsHabits!.answerQuestion5j}," +
                                      $"{m.MFUsHabits!.answerQuestion6},{m.MFUsHabits!.answerQuestion7},{m.MFUsHabits!.answerQuestion8},{m.MFUsHabits!.answerQuestion9}," +
                                      $"{m.resultComponent1},{m.resultComponent2},{m.resultComponent3},{m.resultComponent4},{m.resultComponent5},{m.resultComponent6},{m.resultComponent7}," +
                                      $"{m.globalClassification},{m.classification}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
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

        private async Task<List<DrinkHabit>> GetQueryHabitDrinkAsync(HabitDrinkFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken) 
        {
            List<DrinkHabit> hDrink = new List<DrinkHabit>();

            var query = _bd.HabitsDrink
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

                if (filter.dateHabit != null)
                {
                    query = query.Where(f =>
                        f.drinkDateHabit <= filter.dateHabit &&
                        f.drinkDateHabit >= filter.dateHabit
                    );
                }
            }

            if (!export)
            {
                hDrink = await query
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync(cancellationToken);
            }
            else
            {
                hDrink = await query
                            .Skip(currentPage * 1000)
                            .Take(1000)
                            .ToListAsync(cancellationToken);
            }

            return hDrink;
        }

        private async Task<List<DrugsHabit>> GetQueryHabitDrugAsync(HabitDrugFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken)
        {
            List<DrugsHabit> hDrugs = new List<DrugsHabit>();

            var query = _bd.HabitsDrugs
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

                if (filter.dateHabit != null)
                {
                    query = query.Where(f =>
                        f.drugsDateHabit <= filter.dateHabit &&
                        f.drugsDateHabit >= filter.dateHabit
                    );
                }

                if (!string.IsNullOrWhiteSpace(filter.predominatEmotionalState))
                    query = query.Where(f => f.predominantEmotionalState!.Contains(filter.predominatEmotionalState ?? ""));
            }

            if (!export)
            {
                hDrugs = await query
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync(cancellationToken);
            }
            else
            {
                hDrugs = await query
                            .Skip(currentPage * 1000)
                            .Take(1000)
                            .ToListAsync(cancellationToken);
            }

            return hDrugs;
        }

        private async Task<List<SleepHabit>> GetQueryHabitSleepAsync(HabitSleepFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken)
        {
            List<SleepHabit> hSleep = new List<SleepHabit>();

            var query = _bd.HabitsSleep
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

                if (filter.dateHabit != null)
                {
                    query = query.Where(f =>
                        f.sleepDateHabit <= filter.dateHabit &&
                        f.sleepDateHabit >= filter.dateHabit
                    );
                }

                if (!string.IsNullOrWhiteSpace(filter.perceptionRelax))
                    query = query.Where(f => f.perceptionOfRelaxation!.Contains(filter.perceptionRelax ?? ""));
            }

            if (!export)
            {
                hSleep = await query
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync(cancellationToken);
            }
            else
            {
                hSleep = await query
                            .Skip(currentPage * 1000)
                            .Take(1000)
                            .ToListAsync(cancellationToken);
            }

            return hSleep;
        }

        private async Task<List<HabitsResults>> GetQueryMFUsHabitsAsync(PatientFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken)
        {
            List<HabitsResults> mfu = new List<HabitsResults>();

            var query = _bd.ResultsHabits
                        .Include(rf => rf.MFUsHabits)
                            .ThenInclude(mf => mf!.account)
                        .Include(rf => rf.MFUsHabits)
                            .ThenInclude(mf => mf!.months)
                        .AsQueryable();

            if (filter != null)
            {
                query = query.Where(p => _bd.PacientDoctor
                                          .Where(pd => pd.doctorID == filter!.doctorID)
                                          .Select(pd => pd.accountID)
                                          .Contains(p.MFUsHabits!.account!.accountID));

                if (!string.IsNullOrWhiteSpace(filter!.accountID.ToString()))
                    query = query.Where(f => f.MFUsHabits!.account!.accountID.ToString().Contains(filter.accountID.ToString() ?? ""));

                if (!string.IsNullOrWhiteSpace(filter!.username))
                    query = query.Where(f => f.MFUsHabits!.account!.username.Contains(filter.username ?? ""));

                if (!string.IsNullOrWhiteSpace(filter!.uiemID))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.MFUsHabits!.account!.accountID && p.uiemID == filter.uiemID));

                if (!string.IsNullOrWhiteSpace(filter!.month.ToString())) 
                {
                    var monthStr = Months.VerifyExistMonth(filter?.month ?? 0);
                    query = query.Where(f => f.MFUsHabits!.months!.month.Contains(monthStr));
                }

                if (!string.IsNullOrWhiteSpace(filter!.year.ToString()))
                    query = query.Where(f => f.MFUsHabits!.months!.year == filter.year);

                if (!string.IsNullOrWhiteSpace(filter!.sex))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.MFUsHabits!.account!.accountID && p.sex == filter.sex));

                if (!string.IsNullOrWhiteSpace(filter!.protocolToFollow))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.MFUsHabits!.account!.accountID && p.protocolToFollow == filter.protocolToFollow));
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
