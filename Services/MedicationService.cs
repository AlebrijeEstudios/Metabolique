using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Medication;
using AppVidaSana.GraphicValues;
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

            if (!Save()) { throw new UnstoredValuesException(); }

            if (!(period.initialFrec <= values.dateActual && values.dateActual <= period.finalFrec))
            {
                CreateTimes(period.periodID, period.initialFrec, values.times);

                return null;
            }

            CreateTimes(period.periodID, values.dateActual, values.times);

            return await InfoMedicationAsync(medication, period, values.dateActual, cancellationToken);
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

            var infoMedication = await UpdateForNewDailyFrecAsync(values, period.periodID, cancellationToken);

            if (period.initialFrec != values.initialFrec || period.finalFrec != values.finalFrec)
            {
                infoMedication = await UpdateForNewDateInitialAndFinalAsync(period, values.updateDate,
                                                                            values.initialFrec, values.finalFrec, cancellationToken);
            }

            period.dose = values.dose;

            ValidationValuesDB.ValidationValues(period);

            if (!Save()) { throw new UnstoredValuesException(); }

            var periodUpdate = await _bd.PeriodsMedications.FindAsync(new object[] { values.periodID }, cancellationToken);

            if (periodUpdate is null) { throw new UnstoredValuesException(); }

            if (!(periodUpdate.initialFrec <= values.updateDate && values.updateDate <= periodUpdate.finalFrec))
            {
                return null;
            }

            return infoMedication;
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
            var recordsToDelete = await _bd.Times.Where(e => e.periodID == periodID 
                                                        && e.dateMedication == date).ToListAsync(cancellationToken);

            if (recordsToDelete.Count == 0)
            {
                return "Este registro no existe, inténtelo de nuevo.";
            }

            _bd.Times.RemoveRange(recordsToDelete);

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

            var timesExisting = await _bd.Times.Where(e => e.periodID == periodID
                                                      && period.initialFrec <= e.dateMedication
                                                      && e.dateMedication <= period.finalFrec).ToListAsync(cancellationToken);

            if (timesExisting.Count > 0)
            {
                if (period.datesExcluded == "")
                {
                    period.datesExcluded = date.ToString();
                }else{
                    period.datesExcluded = period.datesExcluded + "," + date.ToString();
                }
            }

            if(timesExisting.Count == 0)
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
                                                                  DateOnly dateRecord, CancellationToken cancellationToken)
        {
            var recordsTimes = await _bd.Times.Where(e => e.periodID == period.periodID
                                                     && e.dateMedication == dateRecord).ToListAsync(cancellationToken);

            var listTimes = _mapper.Map<List<TimeListDto>>(recordsTimes);

            listTimes = listTimes.OrderBy(x => x.time).ToList();

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
            string[] datesExcluded = period.datesExcluded?.Split(',') ?? [];

            var times = await _bd.Times.Where(e => e.periodID == period.periodID
                                              && e.dateMedication == dateActual).ToListAsync(cancellationToken);

            if (times.Count <= 0)
            {
                var days = DatesInRange.GetDatesInRange(period.initialFrec, dateActual);

                foreach (var day in days)
                {
                    var existTimes = await _bd.Times.AnyAsync(e => e.periodID == period.periodID
                                                              && e.dateMedication == day, cancellationToken);

                    if (!existTimes && !datesExcluded.Contains(day.ToString()))
                    {
                        CreateTimes(period.periodID, day, period.timesPeriod);
                    }
                }

                times = await _bd.Times.Where(e => e.periodID == period.periodID
                                              && e.dateMedication == dateActual).ToListAsync(cancellationToken);
            }

            return times;
        }

        private async Task<List<WeeklyAttachmentDto>> WeeklyListAsync(Guid accountID, DateOnly dateActual, CancellationToken cancellationToken)
        {
            int totalMedications = 0, medicationsConsumed = 0;
            List<WeeklyAttachmentDto> weeklyList = new List<WeeklyAttachmentDto>();

            int DayOfWeek = (int) dateActual.DayOfWeek;

            DayOfWeek = DayOfWeek == 0 ? 7 : DayOfWeek;

            DateOnly dateInitial = dateActual.AddDays(-(DayOfWeek - 1));
            DateOnly dateFinal = dateInitial.AddDays(6);

            var timeList = GetTimesForPeriodMedication(accountID);

            var groupTimesByID = timeList.GroupBy(obj => obj.periodID)
                                            .ToDictionary(
                                                g => g.Key,
                                                g => g.ToList()
                                            );

            var dates = DatesInRange.GetDatesInRange(dateInitial, dateFinal);

            foreach (var date in dates)
            {
                foreach (var time in groupTimesByID)
                {
                    var period = await _bd.PeriodsMedications.FindAsync(new object[] { time.Key }, cancellationToken);

                    if (period is null) { throw new UnstoredValuesException(); }

                    var listTimes = time.Value.Where(e => e.dateMedication == date
                                                     && period.initialFrec <= e.dateMedication
                                                     && e.dateMedication <= period.finalFrec).ToList();

                    if (listTimes.Count > 0)
                    {
                        medicationsConsumed = listTimes.Count(e => e.medicationStatus) + medicationsConsumed;

                        totalMedications = listTimes.Count + totalMedications;
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

        private List<Times> GetTimesForPeriodMedication(Guid accountID)
        {
            var getTimes = from pMed in _bd.Set<PeriodsMedications>()
                           join t in _bd.Set<Times>()
                           on pMed.periodID equals t.periodID
                           where pMed.accountID == accountID
                           orderby t.time
                           select t;

            var timeList = getTimes.ToList();

            return timeList;
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

        private void CreateTimes(Guid periodID, DateOnly dateActual, string times)
        {
            List<Times> newTimes = new List<Times>();

            string[] subs = times.Split(',');

            foreach (var sub in subs)
            {
                Times time = new Times
                {
                    periodID = periodID,
                    dateMedication = dateActual,
                    time = TimeOnly.Parse(sub, CultureInfo.InvariantCulture),
                    medicationStatus = false
                };

                ValidationValuesDB.ValidationValues(time);

                newTimes.Add(time);
            }

            _bd.Times.AddRange(newTimes);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private async Task CreateTimesAsync(List<DateOnly> dates, List<TimeOnly> times, Guid periodID, CancellationToken cancellationToken)
        {
            List<Times> newTimes = new List<Times>();

            foreach (var date in dates)
            {
                foreach (var time in times)
                {
                    var timeExist = await _bd.Times.AnyAsync(e => e.periodID == periodID
                                                             && e.dateMedication == date, cancellationToken);

                    if (!timeExist)
                    {

                        Times newTime = new Times
                        {
                            periodID = periodID,
                            dateMedication = date,
                            time = time,
                            medicationStatus = false
                        };

                        ValidationValuesDB.ValidationValues(newTime);

                        newTimes.Add(newTime);
                    }
                }
            }

            _bd.Times.AddRange(newTimes);

            if (!Save()) { throw new UnstoredValuesException(); }
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

        private async Task<InfoMedicationDto?> UpdateForNewDateInitialAndFinalAsync(PeriodsMedications periods, DateOnly dateRecord,
                                                                                    DateOnly newInitialDate, DateOnly newFinalDate,
                                                                                    CancellationToken cancellationToken)
        {
            var medication = await _bd.Medications.FindAsync(new object[] { periods.medicationID }, cancellationToken);

            if (medication is null) { throw new UnstoredValuesException(); }

            if (newFinalDate < newInitialDate) { throw new UnstoredValuesException(); }

            if (newFinalDate < dateRecord || dateRecord < newInitialDate)
            {
                periods.initialFrec = newInitialDate;
                periods.finalFrec = newFinalDate;

                ValidationValuesDB.ValidationValues(periods);

                if (!Save()) { throw new UnstoredValuesException(); }

                return null;
            }

            if (newInitialDate < periods.initialFrec)
            {
                List<TimeOnly> times = new List<TimeOnly>();

                var dates = DatesInRange.GetDatesInRange(newInitialDate, periods.initialFrec.AddDays(-1));

                var timesExample = await _bd.Times.Where(e => e.periodID == periods.periodID
                                                         && e.dateMedication == periods.initialFrec).ToListAsync(cancellationToken);

                times.AddRange(timesExample.Select(e => e.time));

                await CreateTimesAsync(dates, times, periods.periodID, cancellationToken);

                periods.initialFrec = newInitialDate;

                ValidationValuesDB.ValidationValues(periods);

                if (!Save()) { throw new UnstoredValuesException(); }

            }

            if (periods.finalFrec < newFinalDate)
            {
                periods.finalFrec = newFinalDate;

                ValidationValuesDB.ValidationValues(periods);

                if (!Save()) { throw new UnstoredValuesException(); }
            }

            periods.initialFrec = newInitialDate;
            periods.finalFrec = newFinalDate;

            ValidationValuesDB.ValidationValues(periods);

            if (!Save()) { throw new UnstoredValuesException(); }

            return await InfoMedicationAsync(medication, periods, dateRecord, cancellationToken);
        }

        private async Task<InfoMedicationDto> UpdateForNewDailyFrecAsync(UpdateMedicationUseDto values, Guid periodID, CancellationToken cancellationToken)
        {
            var period = await _bd.PeriodsMedications.FindAsync(new object[] { periodID }, cancellationToken);

            if (period is null) { throw new UnstoredValuesException(); }

            var medication = await _bd.Medications.FindAsync(new object[] { period.medicationID }, cancellationToken);

            if (medication is null) { throw new UnstoredValuesException(); }

            Action<List<TimeListDto>, DateOnly> processRecords = (list, date) =>
            {
                foreach (var id in list)
                {
                    var record = _bd.Times.Find(id.timeID);

                    if (record is null) { throw new UnstoredValuesException(); }

                    var recordsToUpdate = _bd.Times.Where(e => e.periodID == periodID
                                                          && e.time == record.time
                                                          && e.dateMedication >= date).ToList();

                    foreach (var val in recordsToUpdate)
                    {
                        val.time = id.time;
                    }

                    _bd.Times.UpdateRange(recordsToUpdate);

                    if (!Save()) { throw new UnstoredValuesException(); }
                }
            };

            if (String.IsNullOrEmpty(values.newTimes))
            {
                processRecords(values.times, values.updateDate);

                await RemoveTimesAsync(periodID, values, cancellationToken);

                return await InfoMedicationAsync(medication, period, values.updateDate, cancellationToken);
            }
            else
            {
                processRecords(values.times, values.updateDate);

                await AddNewTimesAsync(periodID, values, cancellationToken);

                return await InfoMedicationAsync(medication, period, values.updateDate, cancellationToken);
            }
        }

        private async Task RemoveTimesAsync(Guid periodID, UpdateMedicationUseDto values, CancellationToken cancellationToken)
        {
            List<Guid> idsPrevious = new List<Guid>();
            List<Guid> ids = new List<Guid>();

            var recordsTimes = await _bd.Times.Where(e => e.dateMedication == values.updateDate
                                                     && e.periodID == values.periodID).ToListAsync(cancellationToken);

            idsPrevious.AddRange(recordsTimes.Select(e => e.timeID));

            foreach (var item in values.times)
            {
                ids.Add(item.timeID);
            }

            var idsToKeep = idsPrevious.Intersect(ids).ToList();
            var idsToKeepString = string.Join(", ", idsToKeep.Select(id => _bd.Times.Find(id)!.time.ToString("HH:mm")));

            var findIdsToDelete = idsPrevious.Except(ids).ToList();

            foreach (var id in findIdsToDelete)
            {
                var recordTime = await _bd.Times.FindAsync(new object[] { id }, cancellationToken);

                if (recordTime is null) { throw new UnstoredValuesException(); }

                var recordsToDelete = await _bd.Times.Where(e => e.periodID == periodID
                                                            && e.time == recordTime.time
                                                            && e.dateMedication >= values.updateDate).ToListAsync(cancellationToken);

                _bd.Times.RemoveRange(recordsToDelete);

                if (!Save()) { throw new UnstoredValuesException(); }
            }

            var period = await _bd.PeriodsMedications.FindAsync(new object[] { periodID }, cancellationToken);

            if (period is null) { throw new UnstoredValuesException(); }

            period.timesPeriod = idsToKeepString;

            ValidationValuesDB.ValidationValues(period);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private async Task AddNewTimesAsync(Guid periodID, UpdateMedicationUseDto values, CancellationToken cancellationToken)
        {
            List<Times> newTimes = new List<Times>();

            var period = await _bd.PeriodsMedications.FindAsync(new object[] { periodID }, cancellationToken);

            if (period is null) { throw new UnstoredValuesException(); }

            string[] subs = values.newTimes.Split(',');

            foreach (var sub in subs)
            {
                var timeExist = await _bd.Times.AnyAsync(e => e.periodID == periodID
                                                         && e.dateMedication == values.updateDate
                                                         && e.time == TimeOnly.Parse(sub, CultureInfo.InvariantCulture),
                                                         cancellationToken);

                if (!timeExist)
                {
                    Times time = new Times
                    {
                        periodID = periodID,
                        dateMedication = values.updateDate,
                        time = TimeOnly.Parse(sub, CultureInfo.InvariantCulture),
                        medicationStatus = false
                    };

                    ValidationValuesDB.ValidationValues(time);

                    newTimes.Add(time);
                }

                string[] actualTimes = period.timesPeriod.Split(", ");

                if (!actualTimes.Contains(sub))
                {
                    period.timesPeriod = period.timesPeriod + ", " + sub;
                }
            }

            _bd.Times.AddRange(newTimes);

            if (!Save()) { throw new UnstoredValuesException(); }

            ValidationValuesDB.ValidationValues(period);

            if (!Save()) { throw new UnstoredValuesException(); }
        }
    }
}