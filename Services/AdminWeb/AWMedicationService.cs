using AppVidaSana.Data;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWMedicationService : IAWMedication
    {
        private readonly AppDbContext _bd;

        public AWMedicationService(AppDbContext bd)
        {
            _bd = bd;
        }
        public async Task<byte[]> ExportAllPeriodsMedicationsAsync(CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("PeriodID,Medication,AccountID,FrecInitial,FrecFinal,Dose,TimesPeriodActual,DatesExcluded");

                while (currentPage >= 0)
                {
                    var pMed = await _bd.PeriodsMedications
                            .Include(m => m.medication)
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    if (pMed.Count == 0)
                    {
                        break;
                    }

                    foreach (var m in pMed)
                    {
                        var csvLine = $"{m.periodID},{m.medication.nameMedication},{m.accountID},{m.initialFrec},{m.finalFrec}," +
                                      $"{m.dose},\"{m.timesPeriod}\",\"{m.datesExcluded}\"";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllDaysConsumedOfMedAsync(CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("DayConsumedID,PeriodID,Date,ConsuptionTimes");

                while (currentPage >= 0)
                {
                    var days = await _bd.DaysConsumedOfMedications
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    if (days.Count == 0)
                    {
                        break;
                    }

                    foreach (var d in days)
                    {
                        var csvLine = $"{d.dayConsumedID},{d.periodID},{d.dateConsumed},\"{d.consumptionTimes}\"";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllConsumptionTimesAsync(CancellationToken cancellationToken) 
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("TimeID,DayConsumedID,Time,MedicationStatus");

                while (currentPage >= 0)
                {
                    var times = await _bd.Times
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    if (times.Count == 0)
                    {
                        break;
                    }

                    foreach (var t in times)
                    {
                        var csvLine = $"{t.timeID},{t.dayConsumedID},{t.time},{t.medicationStatus}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllMFUsMedicationAsync(CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("MonthlyFollowUpID,AccountID,Month,Year,AnswQ1,AnswQ2,AnswQ3,AnswQ4,StatusAdherence");

                while (currentPage >= 0)
                {
                    var mfus = await _bd.MFUsMedication
                            .Include(m => m.months)
                            .Include(m => m.status)
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    if (mfus.Count == 0)
                    {
                        break;
                    }

                    foreach (var m in mfus)
                    {
                        var csvLine = $"{m.monthlyFollowUpID},{m.accountID},{m.months.month},{m.months.year},{m.answerQuestion1}," +
                                      $"{m.answerQuestion2},{m.answerQuestion3},{m.answerQuestion4},{m.status.statusAdherence}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllSideEffectsAsync(CancellationToken cancellationToken)
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("SideEffectID,AccountID,Date,InitialTime,FinalTime,Description");

                while (currentPage >= 0)
                {
                    var sf = await _bd.SideEffects
                            .Skip(currentPage * pageSize)
                            .Take(pageSize)
                            .ToListAsync(cancellationToken);

                    if (sf.Count == 0)
                    {
                        break;
                    }

                    foreach (var s in sf)
                    {
                        var csvLine = $"{s.sideEffectID},{s.accountID},{s.dateSideEffects},{s.initialTime},{s.finalTime},\"{s.description}\"";

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
