using AppVidaSana.Data;
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
        public async Task<byte[]> ExportAllExercisesAsync(CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("ExerciseID,AccountID,Date,TypeExercise,IntensityExercise,TimeSpent");

                while (currentPage >= 0)
                {
                    var exercises = await _bd.Exercises
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    if (exercises.Count == 0)
                    {
                        break;
                    }

                    foreach (var e in exercises)
                    {
                        var csvLine = $"{e.exerciseID},{e.accountID},{e.dateExercise},{e.typeExercise},{e.intensityExercise},{e.timeSpent}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllActivesMinutesAsync(CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("TimeSpentID,AccountID,Date,Total");

                while (currentPage >= 0)
                {
                    var actMin = await _bd.ActiveMinutes
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    if (actMin.Count == 0)
                    {
                        break;
                    }

                    foreach (var a in actMin)
                    {
                        var csvLine = $"{a.timeSpentID},{a.accountID},{a.dateExercise},{a.totalTimeSpent}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllMFUsExerciseAsync(CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("MonthlyFollowUpID,AccountID,Month,Year,AnswQ1,AnswQ2,AnswQ3,AnswQ4,AnswQ5,AnswQ6,AnswQ7,Act_Walking,Act_Moderate,Act_Vigorous,TotalMET,SendentaryBehavior,LevelAF");

                while (currentPage >= 0)
                {
                    var mfus = await _bd.ResultsExercise
                            .Include(m => m.MFUsExercise)
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
                        var csvLine = $"{m.monthlyFollowUpID},{m.MFUsExercise.accountID},{m.MFUsExercise.months.month},{m.MFUsExercise.months.year},{m.MFUsExercise.question1}," +
                                      $"{m.MFUsExercise.question2},{m.MFUsExercise.question3},{m.MFUsExercise.question4},{m.MFUsExercise.question5}," +
                                      $"{m.MFUsExercise.question6},{m.MFUsExercise.question7}, +" +
                                      $"{m.actWalking},{m.actModerate},{m.actVigorous},{m.totalMET},{m.sedentaryBehavior},{m.levelAF}";

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
