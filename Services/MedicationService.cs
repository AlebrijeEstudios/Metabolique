using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Medication;
using AppVidaSana.Months_Dates;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.Models.Medications;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Sprache;
using System.Data;
using System.Globalization;

namespace AppVidaSana.Services
{
    public class MedicationService : IMedication
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public MedicationService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
        }

        public async Task<InfoMedicationDto?> AddMedicationAsync(AddMedicationUseDto values, CancellationToken cancellationToken)
        {
            var medication = await _bd.Medications.FirstOrDefaultAsync(e => e.nameMedication == values.nameMedication,
                                                                       cancellationToken);

            if (medication is null)
            {
                medication = CreateMedication(values.nameMedication);
            }

            var periodExist = await _bd.PeriodsMedications.AnyAsync(e => e.medicationID == medication.medicationID
                                                                    && e.accountID == values.accountID
                                                                    && e.initialFrec == values.initialFrec
                                                                    && e.finalFrec == values.finalFrec
                                                                    && e.dose == values.dose
                                                                    && e.timesPeriod == values.times, cancellationToken);

            if (periodExist) { throw new NotRepeatPeriodException(); }

            if (values.finalFrec < values.initialFrec) { throw new UnstoredValuesException(); }

            PeriodsMedications period = new PeriodsMedications
            {
                medicationID = medication.medicationID,
                accountID = values.accountID,
                initialFrec = values.initialFrec,
                finalFrec = values.finalFrec,
                dose = values.dose,
                timesPeriod = values.times,
                datesExcluded = ""
            };

            ValidationValuesDB.ValidationValues(period);

            _bd.PeriodsMedications.Add(period);

            DaysConsumedOfMedications dayConsumed = new DaysConsumedOfMedications
            {
                periodID = period.periodID,
                dateConsumed = values.initialFrec,
                consumptionTimes = ""
            };

            ValidationValuesDB.ValidationValues(dayConsumed);

            _bd.DaysConsumedOfMedications.Add(dayConsumed);

            if (!Save()) { throw new UnstoredValuesException(); }

            if (!(period.initialFrec <= values.dateActual && values.dateActual <= period.finalFrec))
            {
                CreateTimes(dayConsumed.dayConsumedID, values.times);

                return null;
            }

            CreateTimes(dayConsumed.dayConsumedID, values.times);

            return await InfoMedicationAsync(medication, period, dayConsumed.dayConsumedID, values.dateActual, cancellationToken);
        }

        public async Task<MedicationsAndValuesGraphicDto> GetMedicationsAsync(Guid accountID, DateOnly dateActual, CancellationToken cancellationToken)
        {
            var periods = await _bd.PeriodsMedications.Where(e => e.accountID == accountID
                                                             && e.initialFrec <= dateActual
                                                             && dateActual <= e.finalFrec).ToListAsync(cancellationToken);

            var listMedications = await ListMedicationsAsync(periods, dateActual, cancellationToken);

            var weeklyList = await WeeklyListAsync(accountID, dateActual, cancellationToken);

            var sideEffects = await SideEffectsAsync(accountID, dateActual, cancellationToken);

            var existMFU = await MFUExistAsync(accountID, dateActual, cancellationToken);

            MedicationsAndValuesGraphicDto medications = new MedicationsAndValuesGraphicDto
            {
                medications = listMedications,
                weeklyAttachments = weeklyList,
                sideEffects = sideEffects,
                mfuStatus = existMFU
            };

            return medications;
        }

        public async Task<InfoMedicationDto?> UpdateMedicationAsync(UpdateMedicationUseDto values, CancellationToken cancellationToken)
        {
            var period = await _bd.PeriodsMedications.FindAsync(new object[] { values.periodID }, cancellationToken);

            if (period is null) { throw new UnstoredValuesException(); }

            await UpdateNameMedicationAsync(period, values, cancellationToken);

            var medication = await _bd.Medications.FindAsync(new object[] { period.medicationID }, cancellationToken);

            if (medication is null) { throw new UnstoredValuesException(); }

            var dayConsumedID = await UpdateForNewDailyFrecAsync(values, period, cancellationToken);

            if (period.initialFrec != values.initialFrec || period.finalFrec != values.finalFrec)
            {
                await UpdateForNewDateInitialAndFinalAsync(period, values, cancellationToken);
            }

            period.dose = values.dose;

            ValidationValuesDB.ValidationValues(period);

            if (!Save()) { throw new UnstoredValuesException(); }

            if (!(period.initialFrec <= values.updateDate && values.updateDate <= period.finalFrec))
            {
                return null;
            }

            return await InfoMedicationAsync(medication, period, dayConsumedID, values.updateDate, cancellationToken);
        }

        public async Task UpdateStatusMedicationAsync(UpdateMedicationStatusDto value, CancellationToken cancellationToken)
        {
            var record = await _bd.Times.FindAsync(new object[] { value.timeID }, cancellationToken);

            if (record is null) { throw new UnstoredValuesException(); }

            record.medicationStatus = value.medicationStatus;

            ValidationValuesDB.ValidationValues(record);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        public async Task<string> DeleteAMedicationAsync(Guid periodID, DateOnly date, CancellationToken cancellationToken)
        {
            var dayConsumed = await _bd.DaysConsumedOfMedications.FirstOrDefaultAsync(e => e.periodID == periodID
                                                                                       && e.dateConsumed == date, cancellationToken);

            if (dayConsumed is null) { return "Este registro no existe, int&eacute;ntelo de nuevo."; }

            _bd.DaysConsumedOfMedications.Remove(dayConsumed);

            if (!Save()) { throw new UnstoredValuesException(); }
            
            await UpdateDatesExcludedAsync(periodID, date, cancellationToken);

            return "Se ha eliminado correctamente.";
        }

        public bool Save()
        {
            try
            {
                return _bd.SaveChanges() >= 0;
            }
            catch (Exception)
            {
                return false;

            }
        }

        private async Task UpdateDatesExcludedAsync(Guid periodID, DateOnly date, CancellationToken cancellationToken)
        {
            var period = await _bd.PeriodsMedications.FindAsync(new object[] { periodID }, cancellationToken);

            if (period is null) { throw new UnstoredValuesException(); }

            if (period.datesExcluded == "")
            {
                period.datesExcluded = date.ToString();
            } else {
                period.datesExcluded = period.datesExcluded + "," + date.ToString();
            }

            string[] datesExcluded = period.datesExcluded?.Split(',') ?? [];

            if (period.initialFrec == date)
            {
                var dates = DatesInRange.GetDatesInRange(date.AddDays(1), period.finalFrec);

                foreach(var newDate in dates)
                {
                    if (!datesExcluded.Contains(newDate.ToString()))
                    {
                        period.initialFrec = newDate;
                        break;
                    }
                }
            }

            if(period.finalFrec == date)
            {
                var dates = DatesInRange.GetDatesInRange(period.initialFrec, date.AddDays(-1)).OrderDescending();

                foreach (var newDate in dates)
                {
                    if (!datesExcluded.Contains(newDate.ToString()))
                    {
                        period.finalFrec = newDate;
                        break;
                    }
                }
            }

            var daysConsumed = await _bd.DaysConsumedOfMedications.Where(e => e.periodID == periodID).ToListAsync(cancellationToken);

            if(daysConsumed.Count == 0)
            { 
                _bd.PeriodsMedications.Remove(period);
            }

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private async Task<bool> MFUExistAsync(Guid accountID, DateOnly dateActual, CancellationToken cancellationToken)
        {
            CultureInfo ci = new CultureInfo("es-ES");
            var monthExist = await _bd.Months.FirstOrDefaultAsync(e => e.month == dateActual.ToString("MMMM", ci)
                                                                  && e.year == Convert.ToInt32(dateActual.ToString("yyyy")),
                                                                  cancellationToken);

            if (monthExist is null) { return false; }

            var mfuExist = await _bd.MFUsMedication.AnyAsync(e => e.accountID == accountID && e.monthID == monthExist.monthID,
                                                             cancellationToken);

            if (!mfuExist) { return false; }

            return true;
        }

        private async Task<InfoMedicationDto> InfoMedicationAsync(Medication medication, PeriodsMedications period, 
                                                                  Guid dayConsumedID, DateOnly date,CancellationToken cancellationToken)
        {
            var recordsTimes = await _bd.Times.Where(e => e.dayConsumedID == dayConsumedID).ToListAsync(cancellationToken);

            var listTimes = _mapper.Map<List<TimeListDto>>(recordsTimes);

            listTimes = listTimes.OrderBy(x => x.time).ToList();

            listTimes.ForEach(e => e.periodID = period.periodID);
            listTimes.ForEach(e => e.dateMedication = date);

            InfoMedicationDto infoMedication = new InfoMedicationDto
            {
                medicationID = medication.medicationID,
                periodID = period.periodID,
                accountID = period.accountID,
                nameMedication = medication.nameMedication,
                dose = period.dose,
                initialFrec = period.initialFrec,
                finalFrec = period.finalFrec,
                times = listTimes
            };

            return infoMedication;
        }

        private async Task<List<InfoMedicationDto>> ListMedicationsAsync(List<PeriodsMedications> periods, DateOnly dateActual,
                                                                         CancellationToken cancellationToken)
        {
            List<InfoMedicationDto> listMedications = new List<InfoMedicationDto>();

            var groupPeriodsByMedicationID = periods.GroupBy(obj => obj.medicationID)
                                                    .ToDictionary(
                                                        g => g.Key,
                                                        g => g.ToList()
                                                    );

            var medicationIDs = groupPeriodsByMedicationID.Keys.ToList();
            var medications = await _bd.Medications.Where(m => medicationIDs.Contains(m.medicationID))
                                                   .ToDictionaryAsync(m => m.medicationID, cancellationToken);

            foreach (var med in groupPeriodsByMedicationID)
            {
                var periodsForMedication = med.Value;

                foreach (var period in periodsForMedication)
                {
                    string[] datesExcluded = period.datesExcluded?.Split(',') ?? [];

                    if (!datesExcluded.Contains(dateActual.ToString()) && period.initialFrec <= dateActual && dateActual <= period.finalFrec)
                    {
                        var times = await TimesForPeriodAsync(period, dateActual, cancellationToken);

                        var timesMapped = _mapper.Map<List<TimeListDto>>(times)
                                                 .OrderBy(x => x.time)
                                                 .ToList();

                        timesMapped.ForEach(e => e.periodID = period.periodID);
                        timesMapped.ForEach(e => e.dateMedication = dateActual);

                        var infoMedication = new InfoMedicationDto
                        {
                            medicationID = med.Key,
                            periodID = period.periodID,
                            accountID = period.accountID,
                            nameMedication = medications[med.Key].nameMedication,
                            dose = period.dose,
                            initialFrec = period.initialFrec,
                            finalFrec = period.finalFrec,
                            times = timesMapped
                        };

                        listMedications.Add(infoMedication);
                    }
                }
            }

            return listMedications;
        }

        private async Task<List<Times>> TimesForPeriodAsync(PeriodsMedications period, DateOnly dateActual,
                                                            CancellationToken cancellationToken)
        {
            var dayConsumed = await CreateDaysConsumedAsync(period, dateActual, cancellationToken);

            var times = await _bd.Times.Where(e => e.dayConsumedID == dayConsumed.dayConsumedID).ToListAsync(cancellationToken);

            if (times.Count == 0)
            {
                if(dayConsumed.consumptionTimes is null || dayConsumed.consumptionTimes == "")
                {
                    return CreateTimes(dayConsumed.dayConsumedID, period.timesPeriod);
                }

                return CreateTimes(dayConsumed.dayConsumedID, dayConsumed.consumptionTimes);
            }

            return times;
        }

        private async Task<DaysConsumedOfMedications> CreateDaysConsumedAsync(PeriodsMedications period, DateOnly dateActual,
                                                                              CancellationToken cancellationToken)
        {
            var dayConsumed = await _bd.DaysConsumedOfMedications.FirstOrDefaultAsync(e => e.periodID == period.periodID
                                                                                      && e.dateConsumed == dateActual, cancellationToken);

            if (dayConsumed is null)
            {
                DaysConsumedOfMedications dayConsumedOfMedication = new DaysConsumedOfMedications
                {
                    periodID = period.periodID,
                    dateConsumed = dateActual,
                    consumptionTimes = ""
                };

                ValidationValuesDB.ValidationValues(dayConsumedOfMedication);

                _bd.DaysConsumedOfMedications.Add(dayConsumedOfMedication);

                if (!Save()) { throw new UnstoredValuesException(); }

                return dayConsumedOfMedication;

            }

            return dayConsumed;
        } 

        private async Task<List<WeeklyAttachmentDto>> WeeklyListAsync(Guid accountID, DateOnly dateActual, CancellationToken cancellationToken)
        {
            int totalMedications = 0, medicationsConsumed = 0;
            List<WeeklyAttachmentDto> weeklyList = new List<WeeklyAttachmentDto>();

            int DayOfWeek = (int) dateActual.DayOfWeek;

            DayOfWeek = DayOfWeek == 0 ? 7 : DayOfWeek;

            DateOnly dateInitial = dateActual.AddDays(-(DayOfWeek - 1));
            DateOnly dateFinal = dateInitial.AddDays(6);

            var dates = DatesInRange.GetDatesInRange(dateInitial, dateFinal);

            var periods = await _bd.PeriodsMedications.Where(e => e.accountID == accountID
                                                             && dates.Any(date => e.initialFrec <= date && date <= e.finalFrec )).ToListAsync(cancellationToken);

            var periodsID = periods.Select(e => e.periodID).ToList();

            var daysConsumed = await _bd.DaysConsumedOfMedications.Where(e => periodsID.Contains(e.periodID))
                                                                  .ToListAsync(cancellationToken);

            var times = await _bd.Times.Where(e => daysConsumed.Select(d => d.dayConsumedID).Contains(e.dayConsumedID))
                                       .ToListAsync(cancellationToken);

            foreach(var date in dates)
            {
                var daysConsumedToPeriod = daysConsumed.Where(e => e.dateConsumed == date
                                                              && !((periods.FirstOrDefault(p => p.periodID == e.periodID)?.datesExcluded ?? "").Split(',')).Contains(date.ToString())).ToList();

                foreach (var day in daysConsumedToPeriod)
                {
                    var timesFromDay = times.Where(e => e.dayConsumedID == day.dayConsumedID).ToList();

                    if (timesFromDay.Count > 0)
                    {
                        medicationsConsumed = timesFromDay.Count(e => e.medicationStatus) + medicationsConsumed;

                        totalMedications = timesFromDay.Count + totalMedications;
                    }
                }

                WeeklyAttachmentDto weeklyAttachment = new WeeklyAttachmentDto
                {
                    date = date,
                    value = medicationsConsumed,
                    limit = totalMedications
                };

                weeklyList.Add(weeklyAttachment);

                totalMedications = 0;
                medicationsConsumed = 0;
            }

            return weeklyList;
        }

        private async Task<List<SideEffectsListDto>> SideEffectsAsync(Guid accountID, DateOnly dateActual, CancellationToken cancellationToken)
        {
            var searchSideEffects = await _bd.SideEffects.Where(e => e.accountID == accountID
                                                                && e.dateSideEffects == dateActual).ToListAsync(cancellationToken);

            List<SideEffectsListDto> sideEffects = _mapper.Map<List<SideEffectsListDto>>(searchSideEffects);

            return sideEffects;

        }

        private Medication CreateMedication(string nameMedication)
        {
            Medication newMedication = new Medication
            {
                nameMedication = nameMedication
            };

            ValidationValuesDB.ValidationValues(newMedication);

            _bd.Medications.Add(newMedication);

            if (!Save()) { throw new UnstoredValuesException(); }

            return newMedication;
        }

        private List<Times> CreateTimes(Guid dayConsumedID, string times)
        {
            List<Times> newTimes = new List<Times>();

            string[] subs = times.Split(',');

            foreach (var sub in subs)
            {
                Times time = new Times
                {
                    dayConsumedID = dayConsumedID,
                    time = TimeOnly.Parse(sub, CultureInfo.InvariantCulture),
                    medicationStatus = false
                };

                ValidationValuesDB.ValidationValues(time);

                newTimes.Add(time);
            }

            _bd.Times.AddRange(newTimes);

            if (!Save()) { throw new UnstoredValuesException(); }

            return newTimes;
        }

        private async Task UpdateNameMedicationAsync(PeriodsMedications period, UpdateMedicationUseDto values, CancellationToken cancellationToken)
        {
            var medication = await _bd.Medications.FindAsync(new object[] { period.medicationID }, cancellationToken);

            if (medication is null) { throw new UnstoredValuesException(); }

            if (medication.nameMedication != values.nameMedication)
            {
                var existNameMedication = await _bd.Medications.FirstOrDefaultAsync(e => e.nameMedication == values.nameMedication,
                                                                                    cancellationToken);

                if (existNameMedication is not null)
                {
                    period.medicationID = existNameMedication.medicationID;

                    ValidationValuesDB.ValidationValues(period);

                    if (!Save()) { throw new UnstoredValuesException(); }
                }
                else
                {
                    CreateMedication(values.nameMedication);

                    var findMedication = await _bd.Medications.FirstOrDefaultAsync(e => e.nameMedication == values.nameMedication,
                                                                                   cancellationToken);

                    if (findMedication is null) { throw new UnstoredValuesException(); }

                    period.medicationID = findMedication.medicationID;

                    ValidationValuesDB.ValidationValues(period);

                    if (!Save()) { throw new UnstoredValuesException(); }
                }
            }
        }

        private async Task UpdateForNewDateInitialAndFinalAsync(PeriodsMedications period, UpdateMedicationUseDto values, CancellationToken cancellationToken)
        {
            string[] datesExcluded = period.datesExcluded?.Split(',').Where(date => !string.IsNullOrWhiteSpace(date)).ToArray() ?? [];

            List<DateOnly> dates = new List<DateOnly>();

            string[] updateDatesExcluded;

            if (values.finalFrec < values.initialFrec) { throw new UnstoredValuesException(); }

            if (values.initialFrec < period.initialFrec)
            {
                dates = DatesInRange.GetDatesInRange(values.initialFrec, period.initialFrec);

                datesExcluded = UpdateDatesExcluded(datesExcluded ?? [], dates);

                period.datesExcluded = string.Join(",", datesExcluded ?? []);
            }

            if (period.finalFrec < values.finalFrec)
            {
                dates = DatesInRange.GetDatesInRange(period.finalFrec, values.finalFrec);

                datesExcluded = UpdateDatesExcluded(datesExcluded ?? [], dates);

                period.datesExcluded = string.Join(",", datesExcluded ?? []);
            }

            if(period.initialFrec < values.initialFrec || values.finalFrec < period.finalFrec)
            {
                dates = DatesInRange.GetDatesInRange(values.initialFrec, values.finalFrec);

                var datesPrevious = DatesInRange.GetDatesInRange(period.initialFrec, period.finalFrec);

                updateDatesExcluded = datesPrevious.Select(e => e.ToString()).Where(date => !dates.Select(e => e.ToString()).Contains(date)).ToArray();

                datesExcluded = ((datesExcluded ?? []).Where(date => !dates.Select(e => e.ToString()).Contains(date))).Union(updateDatesExcluded).ToArray();

                await DeleteDatesExcludedAsync(period.periodID, datesExcluded, cancellationToken);

                period.datesExcluded = string.Join(",", datesExcluded);
            }

            period.initialFrec = values.initialFrec;
            period.finalFrec = values.finalFrec;
            
            ValidationValuesDB.ValidationValues(period);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private static string[] UpdateDatesExcluded(string[] datesExcluded, List<DateOnly> dates)
        {
            var updateDatesExcluded = datesExcluded.Where(date => dates.Select(e => e.ToString()).Contains(date)).ToArray();

            return datesExcluded.Except(updateDatesExcluded).ToArray();
        } 

        private async Task DeleteDatesExcludedAsync(Guid periodID, string[] datesExcluded, CancellationToken cancellationToken)
        {
            var dates = await _bd.DaysConsumedOfMedications.Where(e => e.periodID == periodID
                                                                  && datesExcluded.Select(e => DateOnly.Parse(e, CultureInfo.InvariantCulture)).Contains(e.dateConsumed)).ToListAsync(cancellationToken);

            _bd.DaysConsumedOfMedications.RemoveRange(dates);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private async Task<Guid> UpdateForNewDailyFrecAsync(UpdateMedicationUseDto values, PeriodsMedications period, 
                                                            CancellationToken cancellationToken)
        {
            var dayConsumed = await _bd.DaysConsumedOfMedications.FirstOrDefaultAsync(e => e.periodID == values.periodID
                                                                                      && e.dateConsumed == values.updateDate, cancellationToken);

            if (dayConsumed is null) { throw new UnstoredValuesException(); }

            Action<List<TimeListDto>, DateOnly> processRecords = (times, date) =>
            {
                List<Times> recordsToUpdate = new List<Times>();

                foreach (var t in times)
                {
                    var time = _bd.Times.FirstOrDefault(e => e.dayConsumedID == dayConsumed.dayConsumedID
                                                         && e.timeID == t.timeID);

                    if (time is null) { throw new UnstoredValuesException(); }

                    time.time = t.time;

                    recordsToUpdate.Add(time);
                }

                _bd.Times.UpdateRange(recordsToUpdate);

                if (!Save()) { throw new UnstoredValuesException(); }
            };

            if (String.IsNullOrEmpty(values.newTimes))
            {
                processRecords(values.times, values.updateDate);

                await RemoveTimesAsync(period, dayConsumed, values.times, cancellationToken);

                return dayConsumed.dayConsumedID;
            }
            else
            {
                processRecords(values.times, values.updateDate);

                AddNewTimes(period, dayConsumed, values.times, values.newTimes);

                return dayConsumed.dayConsumedID;
            }
        }

        private async Task RemoveTimesAsync(PeriodsMedications period, DaysConsumedOfMedications dayConsumed, 
                                            List<TimeListDto> times, CancellationToken cancellationToken)
        {
            List<Guid> idsPrevious = new List<Guid>();
            List<Guid> ids = new List<Guid>();

            var joinTimes = string.Join(", ", times.Select(e => e.time.ToString("HH:mm")));

            dayConsumed.consumptionTimes = joinTimes;
            
            var recordsTimes = await _bd.Times.Where(e => e.dayConsumedID == dayConsumed.dayConsumedID).ToListAsync(cancellationToken);

            idsPrevious.AddRange(recordsTimes.Select(e => e.timeID));

            foreach (var item in times)
            {
                ids.Add(item.timeID);
            }

            var findIdsToDelete = idsPrevious.Except(ids).ToList();

            foreach (var timeID in findIdsToDelete)
            {
                var recordToDelete = await _bd.Times.FirstOrDefaultAsync(e => e.dayConsumedID == dayConsumed.dayConsumedID
                                                                         && e.timeID == timeID, cancellationToken);

                if (recordToDelete is null) { throw new UnstoredValuesException(); }

                _bd.Times.Remove(recordToDelete);
            }

            period.timesPeriod = joinTimes;

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private void AddNewTimes(PeriodsMedications period, DaysConsumedOfMedications dayConsumed,
                                 List<TimeListDto> times, string newTimes)
        {
            List<Times> newTimesList = new List<Times>();

            var joinTimes = string.Join(", ", times.Select(e => e.time.ToString("HH:mm")));

            dayConsumed.consumptionTimes = joinTimes;

            string[] subs = newTimes.Split(',');

            foreach (var sub in subs)
            {
                Times time = new Times
                {
                    dayConsumedID = dayConsumed.dayConsumedID,
                    time = TimeOnly.Parse(sub, CultureInfo.InvariantCulture),
                    medicationStatus = false
                };

                ValidationValuesDB.ValidationValues(time);

                newTimesList.Add(time);

                dayConsumed.consumptionTimes = dayConsumed.consumptionTimes + ", " + sub;
            }

            _bd.Times.AddRange(newTimesList);

            ValidationValuesDB.ValidationValues(dayConsumed);

            period.timesPeriod = dayConsumed.consumptionTimes ?? "";

            if (!Save()) { throw new UnstoredValuesException(); }
        }
    }
}