using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Exercise_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
using AppVidaSana.Models.Exercises;
using AppVidaSana.Models.Monthly_Follow_Ups.Results;
using AppVidaSana.Months_Dates;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWExerciseService : IAWExercise
    {
        private readonly AppDbContext _bd;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AWExerciseService(AppDbContext bd, IHttpContextAccessor httpContextAccessor)
        {
            _bd = bd;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<AllExercisesPerUserDto>> GetAllExercisesPerUserAsync(ExerciseFilterDto filter, int page, CancellationToken cancellationToken) 
        {
            var role = UserRole();

            if (role == "Admin") 
            {
                var exercises = await GetQueryExercisesAsync(filter, page, false, 0, cancellationToken);

                var allExercisesPerUser = exercises.Select(ex => new AllExercisesPerUserDto
                {
                    exerciseID = ex.exerciseID,
                    accountID = ex.accountID,
                    username = ex.account!.username,
                    dateExercise = ex.dateExercise,
                    typeExercise = ex.typeExercise,
                    intensityExercise = ex.intensityExercise,
                    timeSpent = ex.timeSpent
                }).ToList();

                return allExercisesPerUser;
            }

            return [];
        }

        public async Task<List<AllActiveMinutesPerExerciseDto>> GetAllActiveMinutesPerExerciseAsync(ActiveMinutesFilterDto filter, int page, CancellationToken cancellationToken) 
        {
            var role = UserRole();

            if (role == "Admin") 
            {
                var actM = await GetQueryActiveMinutesAsync(filter, page, false, 0, cancellationToken);

                var allActMinPerExercise = actM.Select(aM => new AllActiveMinutesPerExerciseDto
                {
                    timeSpentID = aM.timeSpentID,
                    accountID = aM.accountID,
                    username = aM.account!.username,
                    dateExercise = aM.dateExercise,
                    totalTimeSpent = aM.totalTimeSpent
                }).ToList();

                return allActMinPerExercise;
            }

            return [];
        }

        public async Task<List<AllMFUsExercisePerUserDto>> GetMFUsExerciseAsync(PatientFilterDto filter, int page, CancellationToken cancellationToken)
        {
            var role = UserRole();

            if (role == "Admin")
            {
                var mfus = await GetQueryMFUsExerciseAsync(filter, page, false, 0, cancellationToken);

                var allMFUsPerUser = mfus.Select(m => new AllMFUsExercisePerUserDto
                {
                    monthlyFollowUpID = m.monthlyFollowUpID,
                    accountID = m.MFUsExercise!.accountID,
                    username = m.MFUsExercise!.account!.username,
                    month = m.MFUsExercise!.months!.month,
                    year = m.MFUsExercise!.months!.year,
                    answerQuestion1 = m.MFUsExercise!.question1,
                    answerQuestion2 = m.MFUsExercise!.question2,
                    answerQuestion3 = m.MFUsExercise!.question3,
                    answerQuestion4 = m.MFUsExercise!.question4,
                    answerQuestion5 = m.MFUsExercise!.question5,
                    answerQuestion6 = m.MFUsExercise!.question6,
                    answerQuestion7 = m.MFUsExercise!.question7,
                    actWalking = m.actWalking,
                    actModerate = m.actModerate,
                    actVigorous = m.actVigorous,
                    totalMET = m.totalMET,
                    sedentaryBehavior = m.sedentaryBehavior,
                    levelAF = m.levelAF
                }).ToList();

                return allMFUsPerUser;
            }

            return [];
        }


        public async Task<byte[]> ExportAllExercisesAsync(ExerciseFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("ExerciseID,AccountID,Username,DateExercise,TypeExercise,IntensityExercise,TimeSpent");

                while (currentPage >= 0)
                {
                    var exercises = await GetQueryExercisesAsync(filter, 0, true, currentPage, cancellationToken);

                    if (exercises.Count == 0)
                    {
                        break;
                    }

                    foreach (var e in exercises)
                    {
                        var csvLine = $"{e.exerciseID},{e.accountID},{e.account!.username},{e.dateExercise},{e.typeExercise},{e.intensityExercise},{e.timeSpent}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllActivesMinutesAsync(ActiveMinutesFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("TimeSpentID,AccountID,Username,DateExercise,TotalMinutes");

                while (currentPage >= 0)
                {
                    var actMin = await GetQueryActiveMinutesAsync(filter, 0, true, currentPage, cancellationToken);

                    if (actMin.Count == 0)
                    {
                        break;
                    }

                    foreach (var a in actMin)
                    {
                        var csvLine = $"{a.timeSpentID},{a.accountID},{a.account!.username},{a.dateExercise},{a.totalTimeSpent}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllMFUsExerciseAsync(PatientFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("MonthlyFollowUpID,AccountID,Username,Month,Year,AnswQ1,AnswQ2,AnswQ3,AnswQ4,AnswQ5,AnswQ6,AnswQ7,Act_Walking,Act_Moderate,Act_Vigorous,TotalMET,SendentaryBehavior,LevelAF");

                while (currentPage >= 0)
                {
                    var mfus = await GetQueryMFUsExerciseAsync(filter, 0, true, currentPage, cancellationToken);

                    if (mfus.Count == 0)
                    {
                        break;
                    }

                    foreach (var m in mfus)
                    {
                        var csvLine = $"{m.monthlyFollowUpID},{m.MFUsExercise!.accountID},{m.MFUsExercise!.account!.username},{m.MFUsExercise!.months!.month},{m.MFUsExercise!.months!.year},{m.MFUsExercise!.question1}," +
                                      $"{m.MFUsExercise!.question2},{m.MFUsExercise!.question3},{m.MFUsExercise!.question4},{m.MFUsExercise!.question5}," +
                                      $"{m.MFUsExercise!.question6},{m.MFUsExercise!.question7}," +
                                      $"{m.actWalking},{m.actModerate},{m.actVigorous},{m.totalMET},{m.sedentaryBehavior},{m.levelAF}";

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

        private async Task<List<Exercise>> GetQueryExercisesAsync(ExerciseFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken) 
        {
            List<Exercise> exercises = new List<Exercise>();

            var query = _bd.Exercises
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

                if (!string.IsNullOrWhiteSpace(filter.typeExercise))
                    query = query.Where(f => f.typeExercise == filter.typeExercise);

                if (!string.IsNullOrWhiteSpace(filter.intensityExercise))
                    query = query.Where(f => f.intensityExercise == filter.intensityExercise);

                if (filter.dateExercise != null)
                {
                    query = query.Where(f =>
                        f.dateExercise <= filter.dateExercise &&
                        f.dateExercise >= filter.dateExercise
                    );
                }
            }

            if (!export)
            {
                exercises = await query
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync(cancellationToken);
            }
            else
            {
                exercises = await query
                            .Skip(currentPage * 1000)
                            .Take(1000)
                            .ToListAsync(cancellationToken);
            }

            return exercises;
        }

        private async Task<List<ActiveMinutes>> GetQueryActiveMinutesAsync(ActiveMinutesFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken) 
        {
            List<ActiveMinutes> actM = new List<ActiveMinutes>();

            var query = _bd.ActiveMinutes
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

                if (filter.dateExercise != null)
                {
                    query = query.Where(f =>
                        f.dateExercise <= filter.dateExercise &&
                        f.dateExercise >= filter.dateExercise
                    );
                }
            }

            if (!export)
            {
                actM = await query
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync(cancellationToken);
            }
            else
            {
                actM = await query
                            .Skip(currentPage * 1000)
                            .Take(1000)
                            .ToListAsync(cancellationToken);
            }

            return actM;
        }

        private async Task<List<ExerciseResults>> GetQueryMFUsExerciseAsync(PatientFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken)
        {
            List<ExerciseResults> mfu = new List<ExerciseResults>();

            var query = _bd.ResultsExercise
                        .Include(rf => rf.MFUsExercise)
                            .ThenInclude(mf => mf!.account)
                        .Include(rf => rf.MFUsExercise)
                            .ThenInclude(mf => mf!.months)
                        .AsQueryable();

            if (filter != null)
            {
                query = query.Where(p => _bd.PacientDoctor
                                          .Where(pd => pd.doctorID == filter!.doctorID)
                                          .Select(pd => pd.accountID)
                                          .Contains(p.MFUsExercise!.account!.accountID));

                if (!string.IsNullOrWhiteSpace(filter!.accountID.ToString()))
                    query = query.Where(f => f.MFUsExercise!.account!.accountID.ToString().Contains(filter.accountID.ToString() ?? ""));

                if (!string.IsNullOrWhiteSpace(filter!.username))
                    query = query.Where(f => f.MFUsExercise!.account!.username.Contains(filter.username ?? ""));

                if (!string.IsNullOrWhiteSpace(filter!.uiemID))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.MFUsExercise!.account!.accountID && p.uiemID == filter.uiemID));

                if (!string.IsNullOrWhiteSpace(filter!.month.ToString())) 
                {
                    var monthStr = Months.VerifyExistMonth(filter?.month ?? 0);
                    query = query.Where(f => f.MFUsExercise!.months!.month.Contains(monthStr));
                }

                if (!string.IsNullOrWhiteSpace(filter!.year.ToString()))
                    query = query.Where(f => f.MFUsExercise!.months!.year == filter.year);

                if (!string.IsNullOrWhiteSpace(filter!.sex))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.MFUsExercise!.account!.accountID && p.sex == filter.sex));

                if (!string.IsNullOrWhiteSpace(filter!.protocolToFollow))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.MFUsExercise!.account!.accountID && p.protocolToFollow == filter.protocolToFollow));
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
