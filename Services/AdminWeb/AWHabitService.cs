using AppVidaSana.Data;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWHabitService : IAWHabits
    {
        private readonly AppDbContext _bd;

        public AWHabitService(AppDbContext bd)
        {
            _bd = bd;
        }

        public async Task<byte[]> ExportAllHabitsDrinkAsync(CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("DrinkHabitID,AccountID,Date,AmountConsumed");

                while (currentPage >= 0)
                {
                    var hDrink = await _bd.HabitsDrink
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    if (hDrink.Count == 0)
                    {
                        break;
                    }

                    foreach (var h in hDrink)
                    {
                        var csvLine = $"{h.drinkHabitID},{h.accountID},{h.drinkDateHabit},{h.amountConsumed}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllHabitsDrugsAsync(CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("DrugsHabitID,AccountID,Date,CigarettesSmoked,PredominantEmotionalState");

                while (currentPage >= 0)
                {
                    var hDrugs = await _bd.HabitsDrugs
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    if (hDrugs.Count == 0)
                    {
                        break;
                    }

                    foreach (var h in hDrugs)
                    {
                        var csvLine = $"{h.drugsHabitID},{h.accountID},{h.drugsDateHabit},{h.cigarettesSmoked},{h.predominantEmotionalState}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllHabitsSleepAsync(CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("SleepHabitID,AccountID,Date,SleepHours,PerceptionOfRelaxation");

                while (currentPage >= 0)
                {
                    var hSleep = await _bd.HabitsSleep
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    if (hSleep.Count == 0)
                    {
                        break;
                    }

                    foreach (var h in hSleep)
                    {
                        var csvLine = $"{h.sleepHabitID},{h.accountID},{h.sleepDateHabit},{h.sleepHours},{h.perceptionOfRelaxation}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllMFUsHabitsAsync(CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("MonthlyFollowUpID,AccountID,Month,Year,AnswQ1,AnswQ2,AnswQ3,AnswQ4,AnswQ5a,AnswQ5b,AnswQ5c,AnswQ5d,AnswQ5e,AnswQ5f,AnswQ5g,AnswQ5h,AnswQ5i,AnswQ5j,AnswQ6,AnswQ7,AnswQ8,AnswQ9,ResultComponent_1,ResultComponent_2,ResultComponent_3,ResultComponent_4,ResultComponent_5,ResultComponent_6,ResultComponent_7,GlobalClassification,Classification");

                while (currentPage >= 0)
                {
                    var mfus = await _bd.ResultsHabits
                            .Include(m => m.MFUsHabits)
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
                        var csvLine = $"{m.monthlyFollowUpID},{m.MFUsHabits.accountID},{m.MFUsHabits.months.month},{m.MFUsHabits.months.year},{m.MFUsHabits.answerQuestion1}," +
                                      $"{m.MFUsHabits.answerQuestion2},{m.MFUsHabits.answerQuestion3},{m.MFUsHabits.answerQuestion4},{m.MFUsHabits.answerQuestion5a}," +
                                      $"{m.MFUsHabits.answerQuestion5b},{m.MFUsHabits.answerQuestion5c},{m.MFUsHabits.answerQuestion5d},{m.MFUsHabits.answerQuestion5e},{m.MFUsHabits.answerQuestion5f},{m.MFUsHabits.answerQuestion5g},{m.MFUsHabits.answerQuestion5h},{m.MFUsHabits.answerQuestion5i},{m.MFUsHabits.answerQuestion5j}," +
                                      $"{m.MFUsHabits.answerQuestion6},{m.MFUsHabits.answerQuestion7},{m.MFUsHabits.answerQuestion8},{m.MFUsHabits.answerQuestion9}," +
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
    }
}
