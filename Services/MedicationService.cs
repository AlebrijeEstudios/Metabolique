using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Medication;
using AppVidaSana.GraphicValues;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.Models.Medications;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;
using AutoMapper;
using Sprache;
using System.Data;
using System.Globalization;

namespace AppVidaSana.Services
{
    public class MedicationService : IMedication, ISideEffects
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;
        private readonly ValidationValuesDB _validationValues;
        private readonly DatesInRange _datesInRange;

        public MedicationService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
            _validationValues = new ValidationValuesDB();
            _datesInRange = new DatesInRange();
        }

        public InfoMedicationDto? AddMedication(AddMedicationUseDto medication)
        {
            if (!_bd.Accounts.Any(e => e.accountID == medication.accountID)) { throw new UserNotFoundException(); }

            if (!_bd.Medications.Any(e => e.nameMedication == medication.nameMedication))
            {
                CreateMedication(medication.nameMedication);
            }

            var findMedication = _bd.Medications.FirstOrDefault(e => e.nameMedication == medication.nameMedication);

            if (findMedication == null) { throw new UnstoredValuesException(); }

            var periodExist = _bd.PeriodsMedications.Any(e => e.medicationID == findMedication.medicationID
                                                         && e.accountID == medication.accountID
                                                         && e.initialFrec == medication.initialFrec
                                                         && e.finalFrec == medication.finalFrec
                                                         && e.dose == medication.dose
                                                         && e.timesPeriod == medication.times);

            if (periodExist) { throw new NotRepeatPeriodException(); }
            if (medication.finalFrec < medication.initialFrec) { throw new UnstoredValuesException(); }

            PeriodsMedications period = new PeriodsMedications
            {
                medicationID = findMedication.medicationID,
                accountID = medication.accountID,
                initialFrec = medication.initialFrec,
                finalFrec = medication.finalFrec,
                dose = medication.dose,
                timesPeriod = medication.times
            };

            _validationValues.ValidationValues(period);

            _bd.PeriodsMedications.Add(period);

            if (!Save()) { throw new UnstoredValuesException(); }

            var findPeriod = _bd.PeriodsMedications.FirstOrDefault(e => e.accountID == medication.accountID
                                                                  && e.medicationID == findMedication.medicationID
                                                                  && e.initialFrec == medication.initialFrec
                                                                  && e.finalFrec == medication.finalFrec);

            if (findPeriod == null) { throw new UnstoredValuesException(); }

            if (!(period.initialFrec <= medication.dateActual && medication.dateActual <= period.finalFrec))
            {
                CreateTimes(findPeriod.periodID, period.initialFrec, medication.times);

                return null;
            }

            CreateTimes(findPeriod.periodID, medication.dateActual, medication.times);

            var medicationsList = InfoMedicationJustAddUpdateDelete(findMedication.medicationID, findPeriod.periodID, medication.dateActual);

            return medicationsList;

        }

        public MedicationsAndValuesGraphicDto GetMedications(Guid accountID, DateOnly dateActual)
        {
            var periods = _bd.PeriodsMedications.Where(e => e.accountID == accountID
                                                       && e.initialFrec <= dateActual
                                                       && dateActual <= e.finalFrec).ToList();

            var listMedications = ListMedications(periods, dateActual);

            var weeklyList = WeeklyList(accountID, dateActual);

            var sideEffects = SideEffects(accountID, dateActual);

            var existMFU = MFUExist(accountID, dateActual);

            MedicationsAndValuesGraphicDto medications = new MedicationsAndValuesGraphicDto
            {
                medications = listMedications,
                weeklyAttachments = weeklyList,
                sideEffects = sideEffects,
                mfuStatus = existMFU
            };

            return medications;
        }

        public InfoMedicationDto? UpdateMedication(UpdateMedicationUseDto values)
        {
            var period = _bd.PeriodsMedications.Find(values.periodID);

            if (period == null) { throw new UnstoredValuesException(); }

            UpdateNameMedication(period, values);

            var infoMedication = UpdateForNewDailyFrec(values, period.periodID);

            if (period.initialFrec != values.initialFrec || period.finalFrec != values.finalFrec)
            {
                infoMedication = UpdateForNewDateInitialAndFinal(period, values.updateDate,
                                                                 values.initialFrec, values.finalFrec);
            }

            period.dose = values.dose;

            _validationValues.ValidationValues(period);

            _bd.PeriodsMedications.Update(period);

            if (!Save()) { throw new UnstoredValuesException(); }

            var periodUpdate = _bd.PeriodsMedications.Find(values.periodID);

            if (periodUpdate == null) { throw new UnstoredValuesException(); }

            if (!(periodUpdate.initialFrec <= values.updateDate && values.updateDate <= periodUpdate.finalFrec))
            {
                return null;
            }

            return infoMedication;

        }

        public void UpdateStatusMedication(UpdateMedicationStatusDto value)
        {
            var record = _bd.Times.Find(value.timeID);

            if (record == null) { throw new UnstoredValuesException(); }

            record.medicationStatus = value.medicationStatus;

            _validationValues.ValidationValues(record);

            _bd.Times.Update(record);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        public string DeleteAMedication(Guid id, DateOnly date)
        {
            var recordsToDelete = _bd.Times.Where(e => e.periodID == id && e.dateMedication >= date).ToList();

            if (recordsToDelete.Count == 0)
            {
                return "Este registro no existe, inténtelo de nuevo.";
            }

            _bd.Times.RemoveRange(recordsToDelete);

            if (!Save()) { throw new UnstoredValuesException(); }

            return "Se ha eliminado correctamente.";
        }

        public SideEffectsListDto AddSideEffect(AddSideEffectDto values)
        {
            var sideEffectExist = _bd.SideEffects.FirstOrDefault(e => e.accountID == values.accountID
                                                                 && e.dateSideEffects == values.date
                                                                 && e.description == values.description);

            SideEffectsListDto sideEffectMapped;

            if (sideEffectExist != null) { throw new RepeatRegistrationException(); }

            SideEffects sideEffects = new SideEffects
            {
                accountID = values.accountID,
                dateSideEffects = values.date,
                initialTime = values.initialTime,
                finalTime = values.finalTime,
                description = values.description
            };

            _validationValues.ValidationValues(sideEffects);

            _bd.SideEffects.Add(sideEffects);

            if (!Save()) { throw new UnstoredValuesException(); }

            var recentlySideEffect = _bd.SideEffects.FirstOrDefault(e => e.accountID == values.accountID
                                                                    && e.dateSideEffects == values.date
                                                                    && e.description == values.description);

            sideEffectMapped = _mapper.Map<SideEffectsListDto>(recentlySideEffect);

            return sideEffectMapped;
        }

        public SideEffectsListDto UpdateSideEffect(SideEffectsListDto values)
        {
            var sideEffectToUpdate = _bd.SideEffects.Find(values.sideEffectID);

            if (sideEffectToUpdate == null) { throw new UnstoredValuesException(); }

            sideEffectToUpdate.initialTime = values.initialTime;
            sideEffectToUpdate.finalTime = values.finalTime;
            sideEffectToUpdate.description = values.description;

            _validationValues.ValidationValues(sideEffectToUpdate);

            _bd.SideEffects.Update(sideEffectToUpdate);

            if (!Save()) { throw new UnstoredValuesException(); }

            var sideEffectMapped = _mapper.Map<SideEffectsListDto>(sideEffectToUpdate);

            return sideEffectMapped;
        }

        public string DeleteSideEffect(Guid id)
        {
            var sideEffectToDelete = _bd.SideEffects.Find(id);

            if (sideEffectToDelete == null)
            {
                return "Este registro no existe, inténtelo de nuevo.";
            }

            _bd.SideEffects.Remove(sideEffectToDelete);

            if (!Save()) { throw new UnstoredValuesException(); }

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

        private bool MFUExist(Guid accountID, DateOnly dateActual)
        {
            CultureInfo ci = new CultureInfo("es-ES");
            var monthExist = _bd.Months.FirstOrDefault(e => e.month == dateActual.ToString("MMMM", ci)
                                                       && e.year == Convert.ToInt32(dateActual.ToString("yyyy")));

            if (monthExist == null) { return false; }

            var mfuExist = _bd.MFUsMedication.Any(e => e.accountID == accountID
                                              && e.monthID == monthExist.monthID);

            if (!mfuExist) { return false; }

            return true;
        }

        private InfoMedicationDto InfoMedicationJustAddUpdateDelete(Guid medicationID, Guid periodID, DateOnly dateRecord)
        {
            var recordsTimes = _bd.Times.Where(e => e.periodID == periodID
                                               && e.dateMedication == dateRecord).ToList();

            var listTimes = _mapper.Map<List<TimeListDto>>(recordsTimes);

            listTimes = listTimes.OrderBy(x => x.time).ToList();

            var medication = _bd.Medications.Find(medicationID);

            var period = _bd.PeriodsMedications.Find(periodID);

            if (medication == null || period == null) { throw new UnstoredValuesException(); }

            InfoMedicationDto infoMedication = new InfoMedicationDto
            {
                medicationID = medicationID,
                periodID = periodID,
                accountID = period.accountID,
                nameMedication = medication.nameMedication,
                dose = period.dose,
                initialFrec = period.initialFrec,
                finalFrec = period.finalFrec,
                times = listTimes
            };

            return infoMedication;
        }

        private List<InfoMedicationDto> ListMedications(List<PeriodsMedications> periods, DateOnly dateActual)
        {
            List<InfoMedicationDto> listMedications = new List<InfoMedicationDto>();

            List<Times> timesForMedication = new List<Times>();

            var groupPeriodsByMedicationID = periods.GroupBy(obj => obj.medicationID)
                                                    .ToDictionary(
                                                        g => g.Key,
                                                        g => g.Where(e => e.medicationID == g.Key)
                                                    );

            foreach (var med in groupPeriodsByMedicationID)
            {
                var medication = _bd.Medications.Find(med.Key);

                if (medication == null) { throw new UnstoredValuesException(); }

                var periodsForMedication = med.Value;

                foreach (var period in periodsForMedication)
                {
                    var times = TimesForPeriod(period, dateActual);

                    timesForMedication.AddRange(times);

                    var timesMapped = _mapper.Map<List<TimeListDto>>(timesForMedication);

                    timesMapped = timesMapped.OrderBy(x => x.time).ToList();

                    InfoMedicationDto infoMedication = new InfoMedicationDto
                    {
                        medicationID = med.Key,
                        periodID = period.periodID,
                        accountID = period.accountID,
                        nameMedication = medication.nameMedication,
                        dose = period.dose,
                        initialFrec = period.initialFrec,
                        finalFrec = period.finalFrec,
                        times = timesMapped
                    };

                    listMedications.Add(infoMedication);

                    timesForMedication.Clear();
                }
            }

            return listMedications;
        }

        private List<Times> TimesForPeriod(PeriodsMedications period, DateOnly dateActual)
        {
            var times = _bd.Times.Where(e => e.periodID == period.periodID
                                        && e.dateMedication == dateActual).ToList();

            if (times.Count <= 0)
            {
                var days = _datesInRange.GetDatesInRange(period.initialFrec, dateActual);

                foreach (var day in days)
                {
                    var existTimes = _bd.Times.Any(e => e.periodID == period.periodID
                                                   && e.dateMedication == day);

                    if (!existTimes)
                    {
                        CreateTimes(period.periodID, day, period.timesPeriod);
                    }
                }

                times = _bd.Times.Where(e => e.periodID == period.periodID
                                        && e.dateMedication == dateActual).ToList();
            }

            return times;
        }

        private List<WeeklyAttachmentDto> WeeklyList(Guid accountID, DateOnly dateActual)
        {
            int totalMedications = 0, medicationsConsumed = 0;
            List<WeeklyAttachmentDto> weeklyList = new List<WeeklyAttachmentDto>();
            DateOnly dateFinal = dateActual.AddDays(-6);

            var timeList = GetTimesForPeriodMedication(accountID);

            var groupTimesByID = timeList.GroupBy(obj => obj.periodID)
                                            .ToDictionary(
                                                g => g.Key,
                                                g => g.ToList()
                                            );

            List<DateOnly> dates = _datesInRange.GetDatesInRange(dateFinal, dateActual);

            foreach (var date in dates)
            {
                foreach (var time in groupTimesByID)
                {
                    var period = _bd.PeriodsMedications.Find(time.Key);

                    if (period == null) { throw new UnstoredValuesException(); }

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

        private List<SideEffectsListDto> SideEffects(Guid accountID, DateOnly dateActual)
        {
            var searchSideEffects = _bd.SideEffects.Where(e => e.accountID == accountID
                                                    && e.dateSideEffects == dateActual).ToList();

            List<SideEffectsListDto> sideEffects = _mapper.Map<List<SideEffectsListDto>>(searchSideEffects);

            return sideEffects;

        }

        private void CreateMedication(string nameMedication)
        {
            Medication newMedication = new Medication
            {
                nameMedication = nameMedication
            };

            _validationValues.ValidationValues(newMedication);

            _bd.Medications.Add(newMedication);

            if (!Save()) { throw new UnstoredValuesException(); }
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

                _validationValues.ValidationValues(time);

                newTimes.Add(time);
            }

            _bd.Times.AddRange(newTimes);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private void CreateTimes(List<DateOnly> dates, List<TimeOnly> times, Guid periodID)
        {
            List<Times> newTimes = new List<Times>();

            foreach (var date in dates)
            {
                foreach (var time in times)
                {
                    var timeExist = _bd.Times.Any(e => e.periodID == periodID
                                                  && e.dateMedication == date);

                    if (!timeExist)
                    {

                        Times newTime = new Times
                        {
                            periodID = periodID,
                            dateMedication = date,
                            time = time,
                            medicationStatus = false
                        };

                        _validationValues.ValidationValues(newTime);

                        newTimes.Add(newTime);
                    }
                }
            }

            _bd.Times.AddRange(newTimes);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private void UpdateNameMedication(PeriodsMedications period, UpdateMedicationUseDto values)
        {
            var medication = _bd.Medications.Find(period.medicationID);

            if (medication == null) { throw new UnstoredValuesException(); }

            if (medication.nameMedication != values.nameMedication)
            {
                var existNameMedication = _bd.Medications.FirstOrDefault(e => e.nameMedication == values.nameMedication);

                if (existNameMedication != null)
                {
                    period.medicationID = existNameMedication.medicationID;

                    _validationValues.ValidationValues(period);

                    _bd.PeriodsMedications.Update(period);

                    if (!Save()) { throw new UnstoredValuesException(); }
                }
                else
                {
                    CreateMedication(values.nameMedication);

                    var findMedication = _bd.Medications
                                        .FirstOrDefault(e => e.nameMedication == values.nameMedication);

                    if (findMedication == null) { throw new UnstoredValuesException(); }

                    period.medicationID = findMedication.medicationID;

                    _validationValues.ValidationValues(period);

                    _bd.PeriodsMedications.Update(period);

                    if (!Save()) { throw new UnstoredValuesException(); }
                }
            }
        }

        private InfoMedicationDto? UpdateForNewDateInitialAndFinal(PeriodsMedications periods, DateOnly dateRecord,
                                                                   DateOnly newInitialDate, DateOnly newFinalDate)
        {
            if (newFinalDate < newInitialDate) { throw new UnstoredValuesException(); }

            if (newFinalDate < dateRecord || dateRecord < newInitialDate)
            {
                periods.initialFrec = newInitialDate;
                periods.finalFrec = newFinalDate;

                _validationValues.ValidationValues(periods);

                _bd.PeriodsMedications.Update(periods);

                if (!Save()) { throw new UnstoredValuesException(); }

                return null;
            }

            if (newInitialDate < periods.initialFrec)
            {
                List<TimeOnly> times = new List<TimeOnly>();

                var dates = _datesInRange.GetDatesInRange(newInitialDate, periods.initialFrec.AddDays(-1));

                var timesExample = _bd.Times.Where(e => e.periodID == periods.periodID
                                                   && e.dateMedication == periods.initialFrec).ToList();

                times.AddRange(timesExample.Select(e => e.time));

                CreateTimes(dates, times, periods.periodID);

                periods.initialFrec = newInitialDate;

                _validationValues.ValidationValues(periods);

                _bd.PeriodsMedications.Update(periods);

                if (!Save()) { throw new UnstoredValuesException(); }

            }

            if (periods.finalFrec < newFinalDate)
            {
                periods.finalFrec = newFinalDate;

                _validationValues.ValidationValues(periods);

                _bd.PeriodsMedications.Update(periods);

                if (!Save()) { throw new UnstoredValuesException(); }
            }

            periods.initialFrec = newInitialDate;
            periods.finalFrec = newFinalDate;

            _validationValues.ValidationValues(periods);

            _bd.PeriodsMedications.Update(periods);

            if (!Save()) { throw new UnstoredValuesException(); }

            return InfoMedicationJustAddUpdateDelete(periods.medicationID, periods.periodID, dateRecord);
        }

        private InfoMedicationDto UpdateForNewDailyFrec(UpdateMedicationUseDto values, Guid periodID)
        {
            var period = _bd.PeriodsMedications.Find(periodID);

            if (period == null) { throw new UnstoredValuesException(); }

            Action<List<TimeListDto>, DateOnly> processRecords = (list, date) =>
            {
                foreach (var id in list)
                {
                    var record = _bd.Times.Find(id.timeID);

                    if (record == null) { throw new UnstoredValuesException(); }

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

                RemoveTimes(periodID, values);

                return InfoMedicationJustAddUpdateDelete(period.medicationID, periodID, values.updateDate);
            }
            else
            {
                processRecords(values.times, values.updateDate);

                AddNewTimes(periodID, values);

                return InfoMedicationJustAddUpdateDelete(period.medicationID, periodID, values.updateDate);
            }
        }

        private void RemoveTimes(Guid periodID, UpdateMedicationUseDto values)
        {
            List<Guid> idsPrevious = new List<Guid>();
            List<Guid> ids = new List<Guid>();

            var recordsTimes = _bd.Times.Where(e => e.dateMedication == values.updateDate
                                               && e.periodID == values.periodID).ToList();

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
                var recordTime = _bd.Times.Find(id);

                if (recordTime == null) { throw new UnstoredValuesException(); }

                var recordsToDelete = _bd.Times.Where(e => e.periodID == periodID
                                                      && e.time == recordTime.time
                                                      && e.dateMedication >= values.updateDate).ToList();

                _bd.Times.RemoveRange(recordsToDelete);

                if (!Save()) { throw new UnstoredValuesException(); }
            }

            var period = _bd.PeriodsMedications.Find(periodID);

            if (period == null) { throw new UnstoredValuesException(); }

            period.timesPeriod = idsToKeepString;

            _validationValues.ValidationValues(period);

            _bd.PeriodsMedications.Update(period);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private void AddNewTimes(Guid periodID, UpdateMedicationUseDto values)
        {
            List<Times> newTimes = new List<Times>();

            var period = _bd.PeriodsMedications.Find(periodID)!;

            string[] subs = values.newTimes.Split(',');

            foreach (var sub in subs)
            {
                var timeExist = _bd.Times.Any(e => e.periodID == periodID
                                              && e.dateMedication == values.updateDate
                                              && e.time == TimeOnly.Parse(sub, CultureInfo.InvariantCulture));

                if (!timeExist)
                {
                    Times time = new Times
                    {
                        periodID = periodID,
                        dateMedication = values.updateDate,
                        time = TimeOnly.Parse(sub, CultureInfo.InvariantCulture),
                        medicationStatus = false
                    };

                    _validationValues.ValidationValues(time);

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

            _validationValues.ValidationValues(period);

            _bd.PeriodsMedications.Update(period);

            if (!Save()) { throw new UnstoredValuesException(); }
        }
    }
}