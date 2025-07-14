using AppVidaSana.Data;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Exercise_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
using AppVidaSana.Models.Exercises;
using AppVidaSana.Models.Monthly_Follow_Ups.Results;
using AppVidaSana.Months_Dates;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWExerciseService : IAWExercise
    {
        private readonly AppDbContext _bd;

        public AWExerciseService(AppDbContext bd)
        {
            _bd = bd;
        }

        public async Task<List<AllExercisesPerUserDto>> GetAllExercisesPerUserAsync(ExerciseFilterDto filter, int page, CancellationToken cancellationToken) 
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

        public async Task<byte[]> ExportAllExercisesAsync(ExerciseFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;
            List<Exercise> exercises;

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);

            await streamWriter.WriteLineAsync("ExerciseID,AccountID,Username,DateExercise,TypeExercise,IntensityExercise,TimeSpent");

            do
            {
                exercises = await GetQueryExercisesAsync(filter, 0, true, currentPage, cancellationToken);

                foreach (var e in exercises)
                {
                    var csvLine = $"{e.exerciseID},{e.accountID},{e.account!.username},{e.dateExercise},{e.typeExercise},{e.intensityExercise},{e.timeSpent}";
                    await streamWriter.WriteLineAsync(csvLine);
                }

                currentPage++;

            } while (exercises.Count > 0);

            await streamWriter.FlushAsync(cancellationToken);

            return memoryStream.ToArray();
        }
    
        private async Task<List<Exercise>> GetQueryExercisesAsync(ExerciseFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken) 
        {
            List<Exercise> exercises;

            var query = _bd.Exercises
                            .Include(f => f.account)
                            .AsQueryable();

            if (filter != null) 
            {
                query = FilterExercises(query, filter);
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

        private IQueryable<Exercise> FilterExercises(IQueryable<Exercise> query, ExerciseFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.doctorID.ToString()) && filter.doctorID.ToString() != "00000000-0000-0000-0000-000000000000")
                query = query.Where(p => _bd.PacientDoctor
                                          .Where(pd => pd.doctorID == filter.doctorID)
                                          .Select(pd => pd.accountID)
                                          .Contains(p.account!.accountID));

            if (filter.doctorID == Guid.Empty)
            {
                query = query.Where(p => _bd.PacientDoctor
                                    .Where(pd => pd.doctorID == null)
                                    .Select(pd => pd.accountID)
                                    .Contains(p.account!.accountID));
            }

            query = FilterExercisesByPatient(query, filter);

            query = FilterExercisesByExercise(query, filter);

            return query;
        }

        private IQueryable<Exercise> FilterExercisesByPatient(IQueryable<Exercise> query, ExerciseFilterDto filter)
        {
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
                                .Any(p => p.accountID == f.account!.accountID && p.protocol!.protocolToFollow == filter.protocolToFollow));

            return query;
        }

        private static IQueryable<Exercise> FilterExercisesByExercise(IQueryable<Exercise> query, ExerciseFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.typeExercise))
                query = query.Where(f => f.typeExercise == filter.typeExercise);

            if (!string.IsNullOrWhiteSpace(filter.intensityExercise))
                query = query.Where(f => f.intensityExercise == filter.intensityExercise);

            if (filter.startDate != null && filter.endDate != null)
            {
                query = query.Where(f =>
                    f.dateExercise <= filter.endDate &&
                    f.dateExercise >= filter.startDate
                );
            }
            else if (filter.startDate != null)
            {
                query = query.Where(f =>
                    f.dateExercise >= filter.startDate
                );
            }
            else if (filter.endDate != null)
            {
                query = query.Where(f =>
                    f.dateExercise <= filter.endDate
                );
            }

            return query;
        }



        public async Task<List<AllMFUsExercisePerUserDto>> GetMFUsExerciseAsync(PatientFilterDto filter, int page, CancellationToken cancellationToken)
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

        public async Task<byte[]> ExportAllMFUsExerciseAsync(PatientFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;
            List<ExerciseResults> mfus;

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);

            await streamWriter.WriteLineAsync("MonthlyFollowUpID,AccountID,Username,Month,Year,AnswQ1,AnswQ2,AnswQ3,AnswQ4,AnswQ5,AnswQ6,AnswQ7,Act_Walking,Act_Moderate,Act_Vigorous,TotalMET,SendentaryBehavior,LevelAF");

            do
            {
                mfus = await GetQueryMFUsExerciseAsync(filter, 0, true, currentPage, cancellationToken);

                foreach (var m in mfus)
                {
                    var csvLine = $"{m.monthlyFollowUpID},{m.MFUsExercise!.accountID},{m.MFUsExercise!.account!.username},{m.MFUsExercise!.months!.month},{m.MFUsExercise!.months!.year},{m.MFUsExercise!.question1}," +
                                    $"{m.MFUsExercise!.question2},{m.MFUsExercise!.question3},{m.MFUsExercise!.question4},{m.MFUsExercise!.question5}," +
                                    $"{m.MFUsExercise!.question6},{m.MFUsExercise!.question7}," +
                                    $"{m.actWalking},{m.actModerate},{m.actVigorous},{m.totalMET},{m.sedentaryBehavior},{m.levelAF}";

                    await streamWriter.WriteLineAsync(csvLine);
                }

                currentPage++;

            } while (mfus.Count > 0);

            await streamWriter.FlushAsync(cancellationToken);

            return memoryStream.ToArray();
        }

        private async Task<List<ExerciseResults>> GetQueryMFUsExerciseAsync(PatientFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken)
        {
            List<ExerciseResults> mfu;

            var query = _bd.ResultsExercise
                        .Include(rf => rf.MFUsExercise)
                            .ThenInclude(mf => mf!.account)
                        .Include(rf => rf.MFUsExercise)
                            .ThenInclude(mf => mf!.months)
                        .AsQueryable();

            if (filter != null)
            {
                query = FilterMFUsExercise(query, filter);
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

        private IQueryable<ExerciseResults> FilterMFUsExercise(IQueryable<ExerciseResults> query, PatientFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.doctorID.ToString()) && filter.doctorID.ToString() != "00000000-0000-0000-0000-000000000000")
                query = query.Where(p => _bd.PacientDoctor
                                          .Where(pd => pd.doctorID == filter!.doctorID)
                                          .Select(pd => pd.accountID)
                                          .Contains(p.MFUsExercise!.account!.accountID));

            if (filter.doctorID == Guid.Empty)
            {
                query = query.Where(p => _bd.PacientDoctor
                                    .Where(pd => pd.doctorID == null)
                                    .Select(pd => pd.accountID)
                                    .Contains(p.MFUsExercise!.account!.accountID));
            }

            query = FilterMFUsExerciseByPatient(query, filter);

            query = FilterMFUsExerciseByMonthAndYear(query, filter);

            return query;
        }

        private IQueryable<ExerciseResults> FilterMFUsExerciseByPatient(IQueryable<ExerciseResults> query, PatientFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter!.accountID.ToString()))
                query = query.Where(f => f.MFUsExercise!.account!.accountID.ToString().Contains(filter.accountID.ToString() ?? ""));

            if (!string.IsNullOrWhiteSpace(filter!.username))
                query = query.Where(f => f.MFUsExercise!.account!.username.Contains(filter.username ?? ""));

            if (!string.IsNullOrWhiteSpace(filter!.uiemID))
                query = query.Where(f => _bd.Profiles
                                .Any(p => p.accountID == f.MFUsExercise!.account!.accountID && p.uiemID == filter.uiemID));

            if (!string.IsNullOrWhiteSpace(filter!.sex))
                query = query.Where(f => _bd.Profiles
                                .Any(p => p.accountID == f.MFUsExercise!.account!.accountID && p.sex == filter.sex));

            if (!string.IsNullOrWhiteSpace(filter!.protocolToFollow))
                query = query.Where(f => _bd.Profiles
                                .Any(p => p.accountID == f.MFUsExercise!.account!.accountID && p.protocol!.protocolToFollow == filter.protocolToFollow));

            return query;
        }

        private static IQueryable<ExerciseResults> FilterMFUsExerciseByMonthAndYear(IQueryable<ExerciseResults> query, PatientFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter!.month.ToString()))
            {
                var monthStr = Months.VerifyExistMonth(filter?.month ?? 0);
                query = query.Where(f => f.MFUsExercise!.months!.month.Contains(monthStr));
            }

            if (!string.IsNullOrWhiteSpace(filter!.year.ToString()))
                query = query.Where(f => f.MFUsExercise!.months!.year == filter.year);

            return query;
        }



        public async Task<List<AllActiveMinutesPerExerciseDto>> GetAllActiveMinutesPerExerciseAsync(ActiveMinutesFilterDto filter, int page, CancellationToken cancellationToken)
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

        public async Task<byte[]> ExportAllActivesMinutesAsync(ActiveMinutesFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;
            List<ActiveMinutes> actMin;

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);

            await streamWriter.WriteLineAsync("TimeSpentID,AccountID,Username,DateExercise,TotalMinutes");

            do
            {
                actMin = await GetQueryActiveMinutesAsync(filter, 0, true, currentPage, cancellationToken);

                foreach (var actM in actMin)
                {
                    var csvLine = $"{actM.timeSpentID},{actM.accountID},{actM.account!.username},{actM.dateExercise},{actM.totalTimeSpent}";

                    await streamWriter.WriteLineAsync(csvLine);
                }

                currentPage++;

            } while (actMin.Count > 0);

            await streamWriter.FlushAsync(cancellationToken);

            return memoryStream.ToArray();
        }

        private async Task<List<ActiveMinutes>> GetQueryActiveMinutesAsync(ActiveMinutesFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken)
        {
            List<ActiveMinutes> actM = new List<ActiveMinutes>();

            var query = _bd.ActiveMinutes
                            .Include(f => f.account)
                            .AsQueryable();

            if (filter != null)
            {
                query = FilterActiveMinutes(query, filter);
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

        private IQueryable<ActiveMinutes> FilterActiveMinutes(IQueryable<ActiveMinutes> query, ActiveMinutesFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.doctorID.ToString()) && filter.doctorID.ToString() != "00000000-0000-0000-0000-000000000000")
                query = query.Where(p => _bd.PacientDoctor
                                          .Where(pd => pd.doctorID == filter.doctorID)
                                          .Select(pd => pd.accountID)
                                          .Contains(p.account!.accountID));

            if (filter.doctorID == Guid.Empty)
            {
                query = query.Where(p => _bd.PacientDoctor
                                    .Where(pd => pd.doctorID == null)
                                    .Select(pd => pd.accountID)
                                    .Contains(p.account!.accountID));
            }

            query = FilterActiveMinutesByPatient(query, filter);

            query = FilterActiveMinutesByDates(query, filter);

            return query;
        }

        private IQueryable<ActiveMinutes> FilterActiveMinutesByPatient(IQueryable<ActiveMinutes> query, ActiveMinutesFilterDto filter)
        {
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
                                .Any(p => p.accountID == f.account!.accountID && p.protocol!.protocolToFollow == filter.protocolToFollow));

            return query;
        }

        private static IQueryable<ActiveMinutes> FilterActiveMinutesByDates(IQueryable<ActiveMinutes> query, ActiveMinutesFilterDto filter)
        {
            if (filter.startDate != null && filter.endDate != null)
            {
                query = query.Where(f =>
                    f.dateExercise <= filter.endDate &&
                    f.dateExercise >= filter.startDate
                );
            }
            else if (filter.startDate != null)
            {
                query = query.Where(f =>
                    f.dateExercise >= filter.startDate
                );
            }
            else if (filter.endDate != null)
            {
                query = query.Where(f =>
                    f.dateExercise <= filter.endDate
                );
            }

            return query;
        }
    }
}
