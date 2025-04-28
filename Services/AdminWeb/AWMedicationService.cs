using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos;
using AppVidaSana.Models.Medications;
using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Months_Dates;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWMedicationService : IAWMedication
    {
        private readonly AppDbContext _bd;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AWMedicationService(AppDbContext bd, IHttpContextAccessor httpContextAccessor)
        {
            _bd = bd;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<AllPeriodsMedicationsPerUserDto>> GetAllPeriodMedicationsPerUserAsync(PeriodMedicationsFilterDto filter, int page, CancellationToken cancellationToken) 
        {
            var role = UserRole();

            if (role == "Admin") 
            {
                var pMed = await GetQueryPeriodMedicationsAsync(filter, page, false, 0, cancellationToken);

                var allPeriodMedicationPerUser = pMed.Select(pM => new AllPeriodsMedicationsPerUserDto 
                {
                    periodID = pM.daysConsumedOfMedications!.periodMedication!.periodID,
                    accountID = pM.daysConsumedOfMedications!.periodMedication!.account!.accountID,
                    username = pM.daysConsumedOfMedications!.periodMedication!.account!.username,
                    medication = pM.daysConsumedOfMedications!.periodMedication!.medication!.nameMedication,
                    initialDate = pM.daysConsumedOfMedications!.periodMedication!.initialFrec,
                    finalDate = pM.daysConsumedOfMedications!.periodMedication!.finalFrec,
                    daysEliminated = pM.daysConsumedOfMedications!.periodMedication!.datesExcluded ?? "N/A",
                    dose = pM.daysConsumedOfMedications!.periodMedication!.dose,
                    dateConsumed = pM.daysConsumedOfMedications!.dateConsumed,
                    timeConsumed = pM.time,
                    statusConsumed = pM.medicationStatus
                }).ToList();

                return allPeriodMedicationPerUser;
            }

            return [];
        }

        public async Task<List<AllSideEffectsPerUserDto>> GetAllSideEffectsAsync(SideEffectsFilterDto filter, int page, CancellationToken cancellationToken) 
        {
            var role = UserRole();

            if (role == "Admin") 
            {
                var sf = await GetQuerySideEffectsAsync(filter, page, false, 0, cancellationToken);

                var allSideEffectsPerUser = sf.Select(s => new AllSideEffectsPerUserDto 
                {
                    sideEffectID = s.sideEffectID,
                    accountID = s.account!.accountID,
                    username = s.account!.username,
                    date = s.dateSideEffects,
                    initialTime = s.initialTime,
                    finalTime = s.finalTime,
                    description = s.description
                }).ToList();

                return allSideEffectsPerUser;
            }

            return [];
        }

        public async Task<List<AllMFUsMedicationsPerUserDto>> GetMFUsMedicationsAsync(MFUsMedicationFilterDto filter, int page, CancellationToken cancellationToken)
        {
            var role = UserRole();

            if (role == "Admin") 
            {
                var mfus = await GetQueryMFUsMedicationsAsync(filter, page, false, 0, cancellationToken);

                var allMFUsPerUser = mfus.Select(m => new AllMFUsMedicationsPerUserDto
                {
                    monthlyFollowUpID = m.monthlyFollowUpID,
                    accountID = m.account!.accountID,
                    username = m.account!.username,
                    month = m.months!.month,
                    year = m.months!.year,
                    answerQuestion1 = m.answerQuestion1,
                    answerQuestion2 = m.answerQuestion2,
                    answerQuestion3 = m.answerQuestion3,
                    answerQuestion4 = m.answerQuestion4,
                    statusAdherence = m.status!.statusAdherence
                }).ToList();

                return allMFUsPerUser;

            }

            return [];
        }


        public async Task<byte[]> ExportAllPeriodMedicationsAsync(PeriodMedicationsFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("PeriodID,AccountID,Username,Medication,FrecInitial,FrecFinal,DatesExcluded,Dose,DateConsumed,TimeConsumed,StatusConsumed");

                while (currentPage >= 0)
                {
                    var pMed = await GetQueryPeriodMedicationsAsync(filter, 0, true, currentPage, cancellationToken);

                    if (pMed.Count == 0)
                    {
                        break;
                    }

                    foreach (var m in pMed)
                    {
                        var csvLine = $"{m.daysConsumedOfMedications!.periodMedication!.periodID},{m.daysConsumedOfMedications!.periodMedication!.account!.accountID}," +
                                      $"{m.daysConsumedOfMedications!.periodMedication!.account!.username},{m.daysConsumedOfMedications!.periodMedication!.medication!.nameMedication}," +
                                      $"{m.daysConsumedOfMedications!.periodMedication!.initialFrec},{m.daysConsumedOfMedications!.periodMedication!.finalFrec}," +
                                      $"\"{m.daysConsumedOfMedications!.periodMedication!.datesExcluded}\",{m.daysConsumedOfMedications!.periodMedication!.dose}," +
                                      $"{m.daysConsumedOfMedications!.dateConsumed},{m.time},{m.medicationStatus}";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllSideEffectsAsync(SideEffectsFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("SideEffectID,AccountID,Username,Date,InitialTime,FinalTime,Description");

                while (currentPage >= 0)
                {
                    var sf = await GetQuerySideEffectsAsync(filter, 0, true, currentPage, cancellationToken);

                    if (sf.Count == 0)
                    {
                        break;
                    }

                    foreach (var s in sf)
                    {
                        var csvLine = $"{s.sideEffectID},{s.account!.accountID},{s.account!.username},{s.dateSideEffects},{s.initialTime},{s.finalTime},\"{s.description}\"";

                        await streamWriter.WriteLineAsync(csvLine);
                    }
                    currentPage++;
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        public async Task<byte[]> ExportAllMFUsMedicationAsync(MFUsMedicationFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("MonthlyFollowUpID,AccountID,Username,Month,Year,AnswQ1,AnswQ2,AnswQ3,AnswQ4,StatusAdherence");

                while (currentPage >= 0)
                {
                    var mfus = await GetQueryMFUsMedicationsAsync(filter, 0, true, currentPage, cancellationToken);

                    if (mfus.Count == 0)
                    {
                        break;
                    }

                    foreach (var m in mfus)
                    {
                        var csvLine = $"{m.monthlyFollowUpID},{m.accountID},{m.account!.username},{m.months!.month},{m.months!.year},{m.answerQuestion1}," +
                                      $"{m.answerQuestion2},{m.answerQuestion3},{m.answerQuestion4},{m.status!.statusAdherence}";

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

        private async Task<List<Times>> GetQueryPeriodMedicationsAsync(PeriodMedicationsFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken) 
        {
            List<Times> pMed = new List<Times>();

            var query = _bd.Times
                        .Include(t => t.daysConsumedOfMedications)
                            .ThenInclude(dc => dc!.periodMedication)
                                .ThenInclude(p => p!.medication)
                        .Include(t => t.daysConsumedOfMedications)
                            .ThenInclude(dc => dc!.periodMedication)
                                .ThenInclude(p => p!.account)
                        .AsQueryable();

            if (filter != null) 
            {
                query = query.Where(p => _bd.PacientDoctor
                                          .Where(pd => pd.doctorID == filter.doctorID)
                                          .Select(pd => pd.accountID)
                                          .Contains(p.daysConsumedOfMedications!.periodMedication!.account!.accountID));

                if (!string.IsNullOrWhiteSpace(filter.accountID.ToString()))
                    query = query.Where(f => f.daysConsumedOfMedications!.periodMedication!.account!.accountID.ToString().Contains(filter.accountID.ToString() ?? ""));

                if (!string.IsNullOrWhiteSpace(filter.username))
                    query = query.Where(f => f.daysConsumedOfMedications!.periodMedication!.account!.username.Contains(filter.username ?? ""));

                if (!string.IsNullOrWhiteSpace(filter.uiemID))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.daysConsumedOfMedications!.periodMedication!.account!.accountID && p.uiemID == filter.uiemID));

                if (!string.IsNullOrWhiteSpace(filter.month.ToString()))
                    query = query.Where(f => _bd.Profiles
                                 .Any(p => p.accountID == f.daysConsumedOfMedications!.periodMedication!.account!.accountID && p.birthDate.Month == filter.month));

                if (!string.IsNullOrWhiteSpace(filter.year.ToString()))
                    query = query.Where(f => _bd.Profiles
                                 .Any(p => p.accountID == f.daysConsumedOfMedications!.periodMedication!.account!.accountID && p.birthDate.Year == filter.year));

                if (!string.IsNullOrWhiteSpace(filter.sex))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.daysConsumedOfMedications!.periodMedication!.account!.accountID && p.sex == filter.sex));

                if (!string.IsNullOrWhiteSpace(filter.protocolToFollow))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.daysConsumedOfMedications!.periodMedication!.account!.accountID && p.protocolToFollow == filter.protocolToFollow));

                if (!string.IsNullOrWhiteSpace(filter.nameMedication))
                    query = query.Where(f => f.daysConsumedOfMedications!.periodMedication!.medication!.nameMedication == filter.nameMedication);

                if (filter.startDate != null && filter.endDate != null)
                {
                    query = query.Where(f =>
                        f.daysConsumedOfMedications!.dateConsumed <= filter.endDate &&
                        f.daysConsumedOfMedications!.dateConsumed >= filter.startDate
                    );
                }
                else if (filter.startDate != null)
                {
                    query = query.Where(f =>
                        f.daysConsumedOfMedications!.dateConsumed >= filter.startDate
                    );
                }
                else if (filter.endDate != null)
                {
                    query = query.Where(f =>
                        f.daysConsumedOfMedications!.dateConsumed <= filter.endDate
                    );
                }

                if (filter.status != null)
                    query = query.Where(f => f.medicationStatus == filter.status);
            }

            if (!export)
            {
                pMed = await query
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync(cancellationToken);
            }
            else
            {
                pMed = await query
                            .Skip(currentPage * 1000)
                            .Take(1000)
                            .ToListAsync(cancellationToken);
            }

            return pMed;
        }

        private async Task<List<SideEffects>> GetQuerySideEffectsAsync(SideEffectsFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken) 
        {
            List<SideEffects> sideEffects = new List<SideEffects>();

            var query = _bd.SideEffects
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

                if (filter.startDate != null && filter.endDate != null)
                {
                    query = query.Where(f =>
                        f.dateSideEffects <= filter.endDate &&
                        f.dateSideEffects >= filter.startDate
                    );
                }
                else if (filter.startDate != null)
                {
                    query = query.Where(f =>
                        f.dateSideEffects >= filter.startDate
                    );
                }
                else if (filter.endDate != null)
                {
                    query = query.Where(f =>
                        f.dateSideEffects <= filter.endDate
                    );
                }
            }

            if (!export)
            {
                sideEffects = await query
                                .Skip((page - 1) * 10)
                                .Take(10)
                                .ToListAsync(cancellationToken);
            }
            else
            {
                sideEffects = await query
                                .Skip(currentPage * 1000)
                                .Take(1000)
                                .ToListAsync(cancellationToken);
            }

            return sideEffects;
        }

        private async Task<List<MFUsMedication>> GetQueryMFUsMedicationsAsync(MFUsMedicationFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken)
        {
            List<MFUsMedication> mfu = new List<MFUsMedication>();

            var query = _bd.MFUsMedication
                            .Include(m => m.account)
                            .Include(m => m.months)
                            .Include(m => m.status)
                            .AsQueryable();

            if (filter != null)
            {
                query = query.Where(p => _bd.PacientDoctor
                                          .Where(pd => pd.doctorID == filter!.doctorID)
                                          .Select(pd => pd.accountID)
                                          .Contains(p.account!.accountID));

                if (!string.IsNullOrWhiteSpace(filter!.accountID.ToString()))
                    query = query.Where(f => f.account!.accountID.ToString().Contains(filter.accountID.ToString() ?? ""));

                if (!string.IsNullOrWhiteSpace(filter!.username))
                    query = query.Where(f => f.account!.username.Contains(filter.username ?? ""));

                if (!string.IsNullOrWhiteSpace(filter!.uiemID))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.account!.accountID && p.uiemID == filter.uiemID));

                if (!string.IsNullOrWhiteSpace(filter!.month.ToString())) 
                {
                    var monthStr = Months.VerifyExistMonth(filter?.month ?? 0);
                    query = query.Where(f => f.months!.month.Contains(monthStr));
                }
                   
                if (!string.IsNullOrWhiteSpace(filter!.year.ToString()))
                    query = query.Where(f => f.months!.year == filter.year);

                if (!string.IsNullOrWhiteSpace(filter!.sex))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.account!.accountID && p.sex == filter.sex));

                if (!string.IsNullOrWhiteSpace(filter!.protocolToFollow))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.account!.accountID && p.protocolToFollow == filter.protocolToFollow));

                if (!string.IsNullOrWhiteSpace(filter!.statusAdherence))
                    query = query.Where(f => f.status!.statusAdherence.Contains(filter.statusAdherence));
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

    }
}
