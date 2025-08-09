using AppVidaSana.Data;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.Models.Medications;
using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Months_Dates;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWMedicationService : IAWMedication
    {
        private readonly AppDbContext _bd;
        private const string notDoctorID = "00000000-0000-0000-0000-000000000000";

        public AWMedicationService(AppDbContext bd)
        {
            _bd = bd;
        }

        public async Task<List<InfoMedicationDto>> GetAllInfoMedicationsPerUserAsync(PeriodMedicationsFilterDto filter, int page, CancellationToken cancellationToken)
        {
            var meds = await GetQueryInfoMedicationsAsync(filter, page, cancellationToken);

            return meds;
        }

        public async Task<List<AllSideEffectsPerUserDto>> GetAllSideEffectsAsync(SideEffectsFilterDto filter, int page, CancellationToken cancellationToken) 
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

        public async Task<List<AllMFUsMedicationsPerUserDto>> GetMFUsMedicationsAsync(MFUsMedicationFilterDto filter, int page, CancellationToken cancellationToken)
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


        public async Task<byte[]> ExportAllPeriodMedicationsAsync(PeriodMedicationsFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;
            List<Times> pMeds;

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);

            await streamWriter.WriteLineAsync("PeriodID,AccountID,Username,Medication,FrecInitial,FrecFinal,DatesExcluded,Dose,DateConsumed,TimeConsumed,StatusConsumed");

            do
            {
                pMeds = await GetQueryPeriodMedicationsAsync(filter, 0, true, currentPage, cancellationToken);

                foreach (var m in pMeds)
                {
                    var csvLine = $"{m.daysConsumedOfMedications!.periodMedication!.periodID},{m.daysConsumedOfMedications!.periodMedication!.account!.accountID}," +
                                    $"{m.daysConsumedOfMedications!.periodMedication!.account!.username},{m.daysConsumedOfMedications!.periodMedication!.medication!.nameMedication}," +
                                    $"{m.daysConsumedOfMedications!.periodMedication!.initialFrec},{m.daysConsumedOfMedications!.periodMedication!.finalFrec}," +
                                    $"\"{m.daysConsumedOfMedications!.periodMedication!.datesExcluded}\",{m.daysConsumedOfMedications!.periodMedication!.dose}," +
                                    $"{m.daysConsumedOfMedications!.dateConsumed},{m.time},{m.medicationStatus}";

                    await streamWriter.WriteLineAsync(csvLine);
                }

                currentPage++;

            } while (pMeds.Count > 0);

            await streamWriter.FlushAsync(cancellationToken);

            return memoryStream.ToArray();
        }

        public async Task<byte[]> ExportAllSideEffectsAsync(SideEffectsFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;
            List<SideEffects> sideEffects;

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);

            await streamWriter.WriteLineAsync("SideEffectID,AccountID,Username,Date,InitialTime,FinalTime,Description");

            do
            {
                sideEffects = await GetQuerySideEffectsAsync(filter, 0, true, currentPage, cancellationToken);

                foreach (var s in sideEffects)
                {
                    var csvLine = $"{s.sideEffectID},{s.account!.accountID},{s.account!.username},{s.dateSideEffects},{s.initialTime},{s.finalTime},\"{s.description}\"";

                    await streamWriter.WriteLineAsync(csvLine);
                }

                currentPage++;

            } while (sideEffects.Count > 0);

            await streamWriter.FlushAsync(cancellationToken);

            return memoryStream.ToArray();
        }

        public async Task<byte[]> ExportAllMFUsMedicationAsync(MFUsMedicationFilterDto? filter, CancellationToken cancellationToken)
        {
            int currentPage = 0;
            List<MFUsMedication> mfus;

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);

            await streamWriter.WriteLineAsync("MonthlyFollowUpID,AccountID,Username,Month,Year,AnswQ1,AnswQ2,AnswQ3,AnswQ4,StatusAdherence");

            do
            {
                mfus = await GetQueryMFUsMedicationsAsync(filter, 0, true, currentPage, cancellationToken);

                foreach (var m in mfus)
                {
                    var csvLine = $"{m.monthlyFollowUpID},{m.accountID},{m.account!.username},{m.months!.month},{m.months!.year},{m.answerQuestion1}," +
                                    $"{m.answerQuestion2},{m.answerQuestion3},{m.answerQuestion4},{m.status!.statusAdherence}";

                    await streamWriter.WriteLineAsync(csvLine);
                }

                currentPage++;

            } while (mfus.Count > 0);

            await streamWriter.FlushAsync(cancellationToken);

            return memoryStream.ToArray();
        }


        private async Task<List<InfoMedicationDto>> GetQueryInfoMedicationsAsync(PeriodMedicationsFilterDto? filter, int page, CancellationToken cancellationToken)
        {
            List<InfoMedicationDto> meds;

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
                query = FilterInfoMedications(query, filter);
            }

            meds = await query
                    .Where(t => t.daysConsumedOfMedications != null)
                    .GroupBy(t => new
                    {
                        t.daysConsumedOfMedications!.periodMedication!.periodID,
                        t.daysConsumedOfMedications!.dateConsumed
                    })
                    .Skip((page - 1) * 10)
                    .Take(10)
                    .Select(g => new InfoMedicationDto
                    {
                        periodID = g.Key.periodID,
                        medicationID = g.First().daysConsumedOfMedications!.periodMedication!.medicationID,
                        accountID = g.First().daysConsumedOfMedications!.periodMedication!.accountID,
                        nameMedication = g.First().daysConsumedOfMedications!.periodMedication!.medication!.nameMedication,
                        dose = g.First().daysConsumedOfMedications!.periodMedication!.dose,
                        initialFrec = g.First().daysConsumedOfMedications!.periodMedication!.initialFrec,
                        finalFrec = g.First().daysConsumedOfMedications!.periodMedication!.finalFrec,
                        times = g.Select(t => new TimeListDto
                        {
                            timeID = t.timeID,
                            periodID = g.Key.periodID,
                            dateMedication = g.Key.dateConsumed, 
                            time = t.time,
                            medicationStatus = t.medicationStatus
                        }).ToList()
                    })
                    .ToListAsync(cancellationToken);


            return meds;
        }
        
        private async Task<List<Times>> GetQueryPeriodMedicationsAsync(PeriodMedicationsFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken) 
        {
            List<Times> pMed;

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
                query = FilterInfoMedications(query, filter);
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

        private IQueryable<Times> FilterInfoMedications(IQueryable<Times> query, PeriodMedicationsFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.patientFilter!.doctorID.ToString()) && filter.patientFilter!.doctorID.ToString() != notDoctorID)
                query = query.Where(p => _bd.PacientDoctor
                                          .Where(pd => pd.doctorID == filter.patientFilter!.doctorID)
                                          .Select(pd => pd.accountID)
                                          .Contains(p.daysConsumedOfMedications!.periodMedication!.account!.accountID));

            if (filter.patientFilter!.doctorID == Guid.Empty)
            {
                query = query.Where(p => _bd.PacientDoctor
                                    .Where(pd => pd.doctorID == null)
                                    .Select(pd => pd.accountID)
                                    .Contains(p.daysConsumedOfMedications!.periodMedication!.account!.accountID));
            }

            query = FilterInfoMedicationsByPatient(query, filter);

            query = FilterInfoMedicationsByMedication(query, filter);

            return query;
        }

        private IQueryable<Times> FilterInfoMedicationsByPatient(IQueryable<Times> query, PeriodMedicationsFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.patientFilter!.accountID.ToString()))
                query = query.Where(f => f.daysConsumedOfMedications!.periodMedication!.account!.accountID.ToString().Contains(filter.patientFilter!.accountID.ToString() ?? ""));

            if (!string.IsNullOrWhiteSpace(filter.patientFilter!.username))
                query = query.Where(f => f.daysConsumedOfMedications!.periodMedication!.account!.username.Contains(filter.patientFilter!.username ?? ""));

            if (!string.IsNullOrWhiteSpace(filter.patientFilter!.uiemID))
                query = query.Where(f => _bd.Profiles
                                .Any(p => p.accountID == f.daysConsumedOfMedications!.periodMedication!.account!.accountID && p.uiemID == filter.patientFilter!.uiemID));

            if (!string.IsNullOrWhiteSpace(filter.monthYearFilter!.month.ToString()))
                query = query.Where(f => _bd.Profiles
                             .Any(p => p.accountID == f.daysConsumedOfMedications!.periodMedication!.account!.accountID && p.birthDate.Month == filter.monthYearFilter!.month));

            if (!string.IsNullOrWhiteSpace(filter.monthYearFilter!.year.ToString()))
                query = query.Where(f => _bd.Profiles
                             .Any(p => p.accountID == f.daysConsumedOfMedications!.periodMedication!.account!.accountID && p.birthDate.Year == filter.monthYearFilter!.year));

            if (!string.IsNullOrWhiteSpace(filter.patientFilter!.sex))
                query = query.Where(f => _bd.Profiles
                                .Any(p => p.accountID == f.daysConsumedOfMedications!.periodMedication!.account!.accountID && p.sex == filter.patientFilter!.sex));

            if (!string.IsNullOrWhiteSpace(filter.patientFilter!.protocolToFollow))
                query = query.Where(f => _bd.Profiles
                                .Any(p => p.accountID == f.daysConsumedOfMedications!.periodMedication!.account!.accountID && p.protocol!.protocolToFollow == filter.patientFilter!.protocolToFollow));

            return query;
        }

        private static IQueryable<Times> FilterInfoMedicationsByMedication(IQueryable<Times> query, PeriodMedicationsFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.nameMedication))
                query = query.Where(f => f.daysConsumedOfMedications!.periodMedication!.medication!.nameMedication == filter.nameMedication);

            if (filter.datesFilter!.startDate != null && filter.datesFilter!.endDate != null)
            {
                query = query.Where(f =>
                    f.daysConsumedOfMedications!.dateConsumed <= filter.datesFilter!.endDate &&
                    f.daysConsumedOfMedications!.dateConsumed >= filter.datesFilter!.startDate
                );
            }
            else if (filter.datesFilter!.startDate != null)
            {
                query = query.Where(f =>
                    f.daysConsumedOfMedications!.dateConsumed >= filter.datesFilter!.startDate
                );
            }
            else if (filter.datesFilter!.endDate != null)
            {
                query = query.Where(f =>
                    f.daysConsumedOfMedications!.dateConsumed <= filter.datesFilter!.endDate
                );
            }

            if (filter.status != null)
                query = query.Where(f => f.medicationStatus == filter.status);

            return query;
        }


        private async Task<List<SideEffects>> GetQuerySideEffectsAsync(SideEffectsFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken) 
        {
            List<SideEffects> sideEffects;

            var query = _bd.SideEffects
                            .Include(f => f.account)
                            .AsQueryable();

            if (filter != null) 
            {
                query = FilterSideEffects(query, filter);
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

        private IQueryable<SideEffects> FilterSideEffects(IQueryable<SideEffects> query, SideEffectsFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.patientFilter!.doctorID.ToString()) && filter.patientFilter!.doctorID.ToString() != notDoctorID)
                query = query.Where(p => _bd.PacientDoctor
                                        .Where(pd => pd.doctorID == filter.patientFilter!.doctorID)
                                        .Select(pd => pd.accountID)
                                        .Contains(p.account!.accountID));

            if (filter.patientFilter!.doctorID == Guid.Empty)
            {
                query = query.Where(p => _bd.PacientDoctor
                                    .Where(pd => pd.doctorID == null)
                                    .Select(pd => pd.accountID)
                                    .Contains(p.account!.accountID));
            }

            query = FilterSideEffectsByPatient(query, filter);

            query = FilterSideEffectsByDates(query, filter);

            return query;
        }

        private IQueryable<SideEffects> FilterSideEffectsByPatient(IQueryable<SideEffects> query, SideEffectsFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.patientFilter!.accountID.ToString()))
                query = query.Where(f => f.account!.accountID.ToString().Contains(filter.patientFilter!.accountID.ToString() ?? ""));

            if (!string.IsNullOrWhiteSpace(filter.patientFilter!.username))
                query = query.Where(f => f.account!.username.Contains(filter.patientFilter!.username ?? ""));

            if (!string.IsNullOrWhiteSpace(filter.patientFilter!.uiemID))
                query = query.Where(f => _bd.Profiles
                                .Any(p => p.accountID == f.account!.accountID && p.uiemID == filter.patientFilter!.uiemID));

            if (!string.IsNullOrWhiteSpace(filter.monthYearFilter!.month.ToString()))
                query = query.Where(f => _bd.Profiles
                                .Any(p => p.accountID == f.account!.accountID && p.birthDate.Month == filter.monthYearFilter!.month));

            if (!string.IsNullOrWhiteSpace(filter.monthYearFilter!.year.ToString()))
                query = query.Where(f => _bd.Profiles
                                .Any(p => p.accountID == f.account!.accountID && p.birthDate.Year == filter.monthYearFilter!.year));

            if (!string.IsNullOrWhiteSpace(filter.patientFilter!.sex))
                query = query.Where(f => _bd.Profiles
                                .Any(p => p.accountID == f.account!.accountID && p.sex == filter.patientFilter!.sex));

            if (!string.IsNullOrWhiteSpace(filter.patientFilter!.protocolToFollow))
                query = query.Where(f => _bd.Profiles
                                .Any(p => p.accountID == f.account!.accountID && p.protocol!.protocolToFollow == filter.patientFilter!.protocolToFollow));

            return query;
        }

        private static IQueryable<SideEffects> FilterSideEffectsByDates(IQueryable<SideEffects> query, SideEffectsFilterDto filter)
        {
            if (filter.datesFilter!.startDate != null && filter.datesFilter!.endDate != null)
            {
                query = query.Where(f =>
                    f.dateSideEffects <= filter.datesFilter!.endDate &&
                    f.dateSideEffects >= filter.datesFilter!.startDate
                );
            }
            else if (filter.datesFilter!.startDate != null)
            {
                query = query.Where(f =>
                    f.dateSideEffects >= filter.datesFilter!.startDate
                );
            }
            else if (filter.datesFilter!.endDate != null)
            {
                query = query.Where(f =>
                    f.dateSideEffects <= filter.datesFilter!.endDate
                );
            }

            return query;
        }


        private async Task<List<MFUsMedication>> GetQueryMFUsMedicationsAsync(MFUsMedicationFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken)
        {
            List<MFUsMedication> mfu;

            var query = _bd.MFUsMedication
                            .Include(m => m.account)
                            .Include(m => m.months)
                            .Include(m => m.status)
                            .AsQueryable();

            if (filter != null)
            {
                query = FilterMFUsMedications(query, filter);
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

        private IQueryable<MFUsMedication> FilterMFUsMedications(IQueryable<MFUsMedication> query, MFUsMedicationFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter.patientFilter!.doctorID.ToString()) && filter.patientFilter!.doctorID.ToString() != notDoctorID)
                query = query.Where(p => _bd.PacientDoctor
                                          .Where(pd => pd.doctorID == filter!.patientFilter!.doctorID)
                                          .Select(pd => pd.accountID)
                                          .Contains(p.account!.accountID));


            if (filter.patientFilter!.doctorID == Guid.Empty)
            {
                query = query.Where(p => _bd.PacientDoctor
                                    .Where(pd => pd.doctorID == null)
                                    .Select(pd => pd.accountID)
                                    .Contains(p.account!.accountID));
            }

            query = FilterMFUsMedicationsByPatient(query, filter);

            query = FilterMFUsMedicationsByMonthAndYear(query, filter);

            return query;
        }

        private IQueryable<MFUsMedication> FilterMFUsMedicationsByPatient(IQueryable<MFUsMedication> query, MFUsMedicationFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter!.patientFilter!.accountID.ToString()))
                query = query.Where(f => f.account!.accountID.ToString().Contains(filter.patientFilter!.accountID.ToString() ?? ""));

            if (!string.IsNullOrWhiteSpace(filter!.patientFilter!.username))
                query = query.Where(f => f.account!.username.Contains(filter.patientFilter!.username ?? ""));

            if (!string.IsNullOrWhiteSpace(filter!.patientFilter!.uiemID))
                query = query.Where(f => _bd.Profiles
                                .Any(p => p.accountID == f.account!.accountID && p.uiemID == filter.patientFilter!.uiemID));

            if (!string.IsNullOrWhiteSpace(filter!.patientFilter!.sex))
                query = query.Where(f => _bd.Profiles
                                .Any(p => p.accountID == f.account!.accountID && p.sex == filter.patientFilter!.sex));

            if (!string.IsNullOrWhiteSpace(filter!.patientFilter!.protocolToFollow))
                query = query.Where(f => _bd.Profiles
                                .Any(p => p.accountID == f.account!.accountID && p.protocol!.protocolToFollow == filter.patientFilter!.protocolToFollow));

            return query;
        }

        private static IQueryable<MFUsMedication> FilterMFUsMedicationsByMonthAndYear(IQueryable<MFUsMedication> query, MFUsMedicationFilterDto filter)
        {
            if (!string.IsNullOrWhiteSpace(filter!.monthYearFilter!.month.ToString()))
            {
                var monthStr = Months.VerifyExistMonth(filter?.monthYearFilter!.month ?? 0);
                query = query.Where(f => f.months!.month.Contains(monthStr));
            }

            if (!string.IsNullOrWhiteSpace(filter!.monthYearFilter!.year.ToString()))
                query = query.Where(f => f.months!.year == filter.monthYearFilter!.year);

            if (!string.IsNullOrWhiteSpace(filter!.statusAdherence))
                query = query.Where(f => f.status!.statusAdherence.Contains(filter.statusAdherence));

            return query;
        }
    }
}
