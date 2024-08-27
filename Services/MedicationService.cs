using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Medication;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.Models.Medications;
using AppVidaSana.Services.IServices;
using AutoMapper;
using Sprache;
using System.ComponentModel.DataAnnotations;
using System.Data;

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

        public InfoMedicationDto AddMedication(AddMedicationUseDto medication)
        {
            if (!_bd.Accounts.Any(e => e.accountID == medication.accountID)) { throw new UserNotFoundException(); }

            if (!_bd.Medications.Any(e => e.nameMedication == medication.nameMedication)) 
            {
                Medication newMedication = new Medication
                {
                    nameMedication = medication.nameMedication
                };

                ValidationMedication(newMedication);

                _bd.Medications.Add(newMedication);

                if (!Save()) { throw new UnstoredValuesException(); }

            }

            Guid medicationID = _bd.Medications.FirstOrDefault(e => e.nameMedication == medication.nameMedication).medicationID;

            PeriodsMedications period = new PeriodsMedications
            {
                medicationID = medicationID,
                accountID = medication.accountID,
                initialFrec = medication.initialFrec,
                finalFrec = medication.finalFrec,
                dose = medication.dose,
                timesPeriod = medication.times,
                isActive = true
            };

            ValidationPeriodMedication(period);

            _bd.PeriodsMedications.Add(period);

            if (!Save()) { throw new UnstoredValuesException(); }

            Guid periodID = _bd.PeriodsMedications.FirstOrDefault(e => e.medicationID == medicationID
                                                                  && e.initialFrec == medication.initialFrec
                                                                  && e.finalFrec == medication.finalFrec
                                                                  && e.isActive == true).periodID;

            List<Times> newTimes = new List<Times>();

            string[] subs = medication.times.Split(',');

            foreach (var sub in subs)
            {
                Times time = new Times
                {
                    periodID = periodID,
                    dateMedication = medication.date,
                    time = TimeOnly.Parse(sub),
                    medicationStatus = false
                };

                ValidationTime(time);

                newTimes.Add(time);
            }

            _bd.Times.AddRange(newTimes);

            if (!Save()) { throw new UnstoredValuesException(); }

            var medicationsList = InfoMedicationJustAddUpdateDelete(medicationID, periodID, medication.date);

            return medicationsList;

        }

        public MedicationsAndValuesGraphicDto GetMedications(Guid accountID, DateOnly dateActual)
        {
            int countStatus = 0, totalMedications = 0, medicationsConsumed = 0;
            bool statusGeneral = false;
            List<WeeklyAttachmentDto> weeklyList = new List<WeeklyAttachmentDto>();
            DateOnly dateFinal = dateActual.AddDays(-6);

            var periods = _bd.PeriodsMedications.Where(e => e.accountID == accountID
                                                       && e.initialFrec <= dateActual
                                                       && dateActual <= e.finalFrec).ToList();

            List<Guid> periodsID = new List<Guid>();

            periodsID.AddRange(periods.Select(e => e.periodID));

            List<Times> times = new List<Times>();

            foreach(var id in periodsID)
            {
                var recordsTime = _bd.Times.Where(e => e.periodID == id && e.dateMedication >= dateFinal
                                            && e.dateMedication <= dateActual).ToList();

                times.AddRange(recordsTime);
            }

            List<InfoMedicationDto> listMedications = new List<InfoMedicationDto>();

            var timeList = _mapper.Map<List<TimeListDto>>(times);

            timeList = timeList.OrderBy(x => x.time).ToList();

            var groupObjectsByID = timeList.GroupBy(obj => obj.periodID)
                                            .ToDictionary(
                                                g => g.Key,
                                                g => g.ToList()
                                            );

            List<DateOnly> dates = GetDatesInRange(dateFinal, dateActual);

            foreach(var date in dates)
            {
                foreach (var time in groupObjectsByID)
                {
                    countStatus = 0;

                    var period = _bd.PeriodsMedications.Find(time.Key);

                    var med = _bd.Medications.Find(period.medicationID);

                    var list = time.Value.Where(e => e.dateMedication == date).ToList();

                    if (list.Any())
                    {
                        totalMedications++;

                        foreach (var l in list)
                        {
                            if (l.medicationStatus) { countStatus++; }
                        }

                        statusGeneral = (countStatus == list.Count()) ? true : false;

                        if (statusGeneral) { medicationsConsumed++; }
                    }
                }

                WeeklyAttachmentDto weeklyAttachment = new WeeklyAttachmentDto
                {
                    date = date,
                    medicationsConsumed = medicationsConsumed,
                    totalMedications = totalMedications
                };

                weeklyList.Add(weeklyAttachment);

                totalMedications = 0;
                medicationsConsumed = 0;
            }

            /*var recordsTimesActual = _bd.Times.Where(e => e.accountID == id 
                                                    && e.dateMedication == dateActual).ToList();

            var listTimesActual = _mapper.Map<List<TimeListDto>>(recordsTimesActual);

            listTimesActual = listTimesActual.OrderBy(x => x.time).ToList();

            var groupObjectsByIDActual = listTimesActual.GroupBy(obj => obj.periodID)
                                                        .ToDictionary(
                                                            g => g.Key,
                                                            g => g.ToList()
                                                        );

            foreach (var time in groupObjectsByIDActual)
            {
                var period = _bd.PeriodsMedications.Find(time.Key);
                var list = time.Value;

                var medication = _bd.Medications.Find(period.medicationID);

                if (list.Any())
                {
                    InfoMedicationDto infoMedication = new InfoMedicationDto
                    {
                        medicationID = medication.medicationID,
                        periodID = time.Key,
                        accountID = medication.accountID,
                        nameMedication = medication.nameMedication,
                        dose = medication.dose,
                        initialFrec = period.initialFrec,
                        finalFrec = period.finalFrec,
                        times = list
                    };

                    listMedications.Add(infoMedication);
                }
            }*/

            MedicationsAndValuesGraphicDto medications = new MedicationsAndValuesGraphicDto
            {
                medications = listMedications,
                weeklyAttachments = weeklyList
            };

            return medications;
        }

        public InfoMedicationDto UpdateMedication(UpdateMedicationUseDto values)
        {
            InfoMedicationDto infoMedication = new InfoMedicationDto();

            var medication = _bd.Medications.Find(values.medicationID);

            if (medication == null) { throw new UnstoredValuesException(); }

            var period = _bd.PeriodsMedications.Find(values.periodID);

            infoMedication = UpdateForNewDailyFrec(values);
            
            if (period.initialFrec != values.initialFrec || period.finalFrec != values.finalFrec)
            {
                infoMedication =  UpdateForNewDateInitialAndFinal(medication, period, values.dateRecord, values.initialFrec, values.finalFrec);
            }

            medication.nameMedication = values.nameMedication;
            //medication.dose = values.dose;

            ValidationMedication(medication);

            _bd.Medications.Update(medication);

            if (!Save()) { throw new UnstoredValuesException(); }

            return infoMedication;

        }

        public void UpdateStatusMedication(UpdateMedicationStatusDto value)
        {
            var record = _bd.Times.Find(value.timeID);

            record.medicationStatus = value.medicationStatus;

            _bd.Times.Update(record);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        public string DeleteAMedication(Guid id, DateOnly date)
        {
            Action<Guid, DateOnly> processRecords = (periodID, dateRecord) =>
            {
                var records = _bd.Times.Where(e => e.periodID == periodID && e.dateMedication == dateRecord).ToList();
               
                _bd.Times.RemoveRange(records);

                if (!Save()) { throw new UnstoredValuesException(); }
            };

            var recordToDelete = _bd.PeriodsMedications.FirstOrDefault(e => e.medicationID == id
                                                        && e.initialFrec < date && date < e.finalFrec);

            var updateInitialFrec = _bd.PeriodsMedications.Any(e => e.medicationID == id
                                                               && e.initialFrec == date && e.finalFrec != date);

            var updateFinalFrec = _bd.PeriodsMedications.Any(e => e.medicationID == id
                                                             && e.initialFrec != date && e.finalFrec == date);

            var lastRecord = _bd.PeriodsMedications.Any(e => e.medicationID == id
                                                        && e.initialFrec == date && e.finalFrec == date);

            if(recordToDelete != null)
            {
                processRecords(recordToDelete.periodID, date);
            }

            if (updateInitialFrec)
            {
                var recordToUpdateInitialFrec = _bd.PeriodsMedications.FirstOrDefault(e => e.medicationID == id
                                                                                      && e.initialFrec == date && e.finalFrec != date);

                recordToUpdateInitialFrec.initialFrec = recordToUpdateInitialFrec.initialFrec.AddDays(1);

                _bd.PeriodsMedications.Update(recordToUpdateInitialFrec);

                if (!Save()) { throw new UnstoredValuesException(); }

                processRecords(recordToUpdateInitialFrec.periodID, date);
            }

            if (updateFinalFrec)
            {
                var recordToUpdateFinalFrec = _bd.PeriodsMedications.FirstOrDefault(e => e.medicationID == id
                                                                                    && e.initialFrec != date && e.finalFrec == date);

                recordToUpdateFinalFrec.finalFrec = recordToUpdateFinalFrec.finalFrec.AddDays(-1);

                _bd.PeriodsMedications.Update(recordToUpdateFinalFrec);

                if (!Save()) { throw new UnstoredValuesException(); }

                processRecords(recordToUpdateFinalFrec.periodID, date);
            }

            if(lastRecord)
            {
                var totalPeriods = _bd.PeriodsMedications.Count(e => e.medicationID == id);

                var record = _bd.PeriodsMedications.FirstOrDefault(e => e.medicationID == id
                                                        && e.initialFrec == date && e.finalFrec == date);

                processRecords(record.periodID, date);

                var recordsToDelete = _bd.Times.Where(e => e.periodID == record.periodID).ToList();

                _bd.Times.RemoveRange(recordsToDelete);
                
                if (!Save()) { throw new UnstoredValuesException(); }

                _bd.PeriodsMedications.Remove(record);

                if (!Save()) { throw new UnstoredValuesException(); }

                if (totalPeriods == 1)
                {
                    _bd.Medications.Remove(_bd.Medications.Find(id));

                    if (!Save()) { throw new UnstoredValuesException(); }
                }
            }

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

        private InfoMedicationDto InfoMedicationJustAddUpdateDelete(Guid medicationID, Guid periodID, DateOnly dateRecord)
        {
            var recordsTimes = _bd.Times.Where(e => e.periodID == periodID
                                               && e.dateMedication == dateRecord).ToList();

            var listTimes = _mapper.Map<List<TimeListDto>>(recordsTimes);

            listTimes = listTimes.OrderBy(x => x.time).ToList();

            var medication = _bd.Medications.Find(medicationID);

            var period = _bd.PeriodsMedications.Find(periodID);

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

        private void ValidationMedication(Medication med)
        {
            var valResults = new List<ValidationResult>();
            var valContext = new ValidationContext(med, null, null);

            if (!Validator.TryValidateObject(med, valContext, valResults, true))
            {
                var errors = valResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }
        }

        private void ValidationPeriodMedication(PeriodsMedications periods)
        {
            var valResults = new List<ValidationResult>();
            var valContext = new ValidationContext(periods, null, null);

            if (!Validator.TryValidateObject(periods, valContext, valResults, true))
            {
                var errors = valResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }
        }

        private void ValidationTime(Times time)
        {
            var valResults = new List<ValidationResult>();
            var valContext = new ValidationContext(time, null, null);

            if (!Validator.TryValidateObject(time, valContext, valResults, true))
            {
                var errors = valResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }
        }

        private static List<DateOnly> GetDatesInRange(DateOnly startDate, DateOnly endDate)
        {
            List<DateOnly> dates = new List<DateOnly>();

            if (endDate >= startDate)
            {
                for (DateOnly date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    dates.Add(date);
                }
            }

            if (endDate < startDate)
            {
                throw new ErrorRangeDatesException();
            }

            return dates;
        }

        private void AddTimes(Guid periodID, Guid accountID, List<DateOnly> dates, List<TimeOnly> times)
        {
            if (!times.Any())
            {
                throw new ListTimesVoidException();
            }

            foreach (DateOnly date in dates)
            {
                foreach (TimeOnly time in times)
                {
                    Times register = new Times
                    {
                        periodID = periodID,
                        //accountID = accountID,
                        dateMedication = date,
                        time = time,
                        medicationStatus = false
                    };

                    var validationResults = new List<ValidationResult>();
                    var validationContext = new ValidationContext(register, null, null);

                    if (!Validator.TryValidateObject(register, validationContext, validationResults, true))
                    {
                        var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                        if (errors.Count > 0)
                        {
                            throw new ErrorDatabaseException(errors);
                        }
                    }

                    _bd.Times.Add(register);

                    if (!Save()) { throw new UnstoredValuesException(); }
                }
            }
        }

        private InfoMedicationDto UpdateForNewDateInitialAndFinal(Medication medication, PeriodsMedications periods, 
                                                                  DateOnly dateRecord, DateOnly newInitialDate, 
                                                                  DateOnly newFinalDate)
        {
            var idsPeriods = _bd.PeriodsMedications.Where(e => e.medicationID == medication.medicationID
                                                          && e.isActive == false).ToList();

            List<Guid> idsToRemove = new List<Guid>();

            idsToRemove.AddRange(idsPeriods.Select(e => e.periodID));

            Action<List<Guid>> processRecords = (listID) =>
            {
                foreach (var id in listID)
                {
                    var recordsToDelete = _bd.PeriodsMedications.Where(e => e.periodID == id).ToList();

                    _bd.PeriodsMedications.RemoveRange(recordsToDelete);

                    if (!Save()) { throw new UnstoredValuesException(); }
                }
            };

            var recordExisting = _bd.PeriodsMedications.Any(e => e.medicationID ==  periods.medicationID
                                                            && e.initialFrec == newInitialDate
                                                            && e.finalFrec ==  newFinalDate && e.isActive == false);

            if (recordExisting)
            {
                List<TimeOnly> timesToDateDefault = new List<TimeOnly>();
                List<Guid> idsPeriodsToDelete = new List<Guid>();

                var times = _bd.Times.Where(e => e.periodID == periods.periodID
                                            && e.dateMedication == dateRecord).ToList();

                timesToDateDefault.AddRange(times.Select(e => e.time));

                var deletePeriods = _bd.PeriodsMedications.Where(e => e.medicationID == medication.medicationID).ToList();
                
                foreach(var obj in deletePeriods)
                {
                    var records = _bd.Times.Where(e => e.periodID == obj.periodID).ToList();

                    _bd.Times.RemoveRange(records);

                    if (!Save()) { throw new UnstoredValuesException(); }
                }

                var lastPeriod = _bd.PeriodsMedications.Where(e => e.medicationID == periods.medicationID
                                                            && e.initialFrec == newInitialDate
                                                            && e.finalFrec == newFinalDate && e.isActive == false).ToList();
                
                List<Guid> idToReturn = new List<Guid>();

                idToReturn.AddRange(lastPeriod.Select(e => e.periodID));

                idsPeriodsToDelete.AddRange(deletePeriods.Select(e => e.periodID));

                var ids = idsPeriodsToDelete.Except(idToReturn).ToList();

                processRecords(ids);

                foreach (var item in lastPeriod)
                {
                    item.isActive = true;

                    _bd.PeriodsMedications.Update(item);

                    if (!Save()) { throw new UnstoredValuesException(); }

                    List<DateOnly> dates = GetDatesInRange(newInitialDate, newFinalDate);

                    //AddTimes(item.periodID, medication.accountID, dates, timesToDateDefault);

                    return InfoMedicationJustAddUpdateDelete(item.medicationID, item.periodID, dateRecord);
                }
            }

            if (periods.initialFrec < newInitialDate && newFinalDate < periods.finalFrec)
            {
                periods.isActive = false;

                ValidationPeriodMedication(periods);

                _bd.PeriodsMedications.Update(periods);

                if (!Save()) { throw new UnstoredValuesException(); }

                PeriodsMedications newPeriod = new PeriodsMedications
                {
                    medicationID = medication.medicationID,
                    initialFrec = newInitialDate,
                    finalFrec = newFinalDate,
                    isActive = true
                };

                ValidationPeriodMedication(newPeriod);

                _bd.PeriodsMedications.Add(newPeriod);

                if (!Save()) { throw new UnstoredValuesException(); }

                var recentlyPeriodMedication = _bd.PeriodsMedications.
                                                FirstOrDefault(e => e.medicationID == medication.medicationID
                                                               && e.initialFrec == newInitialDate
                                                               && e.finalFrec == newFinalDate && e.isActive == true);

                var records = _bd.Times.Where(e => e.dateMedication >= newInitialDate
                                              && e.dateMedication <= newFinalDate
                                              && e.periodID == periods.periodID).ToList();

                foreach (var item in records)
                {
                    item.periodID = recentlyPeriodMedication.periodID;
                }

                _bd.Times.UpdateRange(records);

                _bd.Medications.Update(medication);

                if (!Save()) { throw new UnstoredValuesException(); }

                if (dateRecord < newInitialDate || newFinalDate < dateRecord)
                {
                    return InfoMedicationJustAddUpdateDelete(periods.medicationID, periods.periodID, dateRecord);
                }

                return InfoMedicationJustAddUpdateDelete(recentlyPeriodMedication.medicationID, recentlyPeriodMedication.periodID, dateRecord);
            }

            if(periods.initialFrec == newInitialDate && newFinalDate < periods.finalFrec || periods.initialFrec < newInitialDate && newFinalDate == periods.finalFrec)
            {
                periods.isActive = false;

                ValidationPeriodMedication(periods);

                _bd.PeriodsMedications.Update(periods);

                if (!Save()) { throw new UnstoredValuesException(); }

                PeriodsMedications newPeriod = new PeriodsMedications
                {
                    medicationID = medication.medicationID,
                    initialFrec = newInitialDate,
                    finalFrec = newFinalDate,
                    isActive = true
                };

                ValidationPeriodMedication(newPeriod);

                _bd.PeriodsMedications.Add(newPeriod);

                if (!Save()) { throw new UnstoredValuesException(); }

                var recentlyPeriodMedication = _bd.PeriodsMedications.
                                                FirstOrDefault(e => e.medicationID == medication.medicationID
                                                               && e.initialFrec == newInitialDate
                                                               && e.finalFrec == newFinalDate && e.isActive == true);

                var records = _bd.Times.Where(e => e.dateMedication >= newInitialDate
                                              && e.dateMedication <= newFinalDate
                                              && e.periodID == periods.periodID).ToList();

                foreach (var item in records)
                {
                    item.periodID = recentlyPeriodMedication.periodID;
                }

                _bd.Times.UpdateRange(records);

                _bd.Medications.Update(medication);

                if (!Save()) { throw new UnstoredValuesException(); }

                if(dateRecord < newInitialDate || newFinalDate < dateRecord)
                {
                    return InfoMedicationJustAddUpdateDelete(medication.medicationID, periods.periodID, dateRecord);
                }

                return InfoMedicationJustAddUpdateDelete(recentlyPeriodMedication.medicationID, recentlyPeriodMedication.periodID, dateRecord);
            }

            if (newInitialDate < periods.initialFrec)
            {
                foreach (var id in idsToRemove) 
                {
                    var recordsToDelete = _bd.Times.Where(e => e.periodID == id
                                                          && e.dateMedication >= newInitialDate
                                                          && e.dateMedication <= periods.initialFrec.AddDays(-1)).ToList();

                    _bd.Times.RemoveRange(recordsToDelete);

                    if (!Save()) { throw new UnstoredValuesException(); }

                }

                var recordToUpdate = _bd.PeriodsMedications.Find(periods.periodID);

                List<TimeOnly> timesPrevious = new List<TimeOnly>();

                var newRecordsToAList = GetDatesInRange(newInitialDate, periods.initialFrec.AddDays(-1));

                var recordsExamples = _bd.Times.Where(e => e.periodID == periods.periodID
                                                    && e.dateMedication == periods.initialFrec).ToList();

                timesPrevious.AddRange(recordsExamples.Select(e => e.time));

                //AddTimes(periods.periodID, medication.accountID, newRecordsToAList, timesPrevious);

                recordToUpdate.initialFrec = newInitialDate;

                _bd.PeriodsMedications.Update(recordToUpdate);

                if (!Save()) { throw new UnstoredValuesException(); }

            }

            if (periods.finalFrec < newFinalDate)
            {
                foreach (var id in idsToRemove)
                {
                    var recordsToDelete = _bd.Times.Where(e => e.periodID == id
                                                          && e.dateMedication >= periods.finalFrec.AddDays(1)
                                                          && e.dateMedication <= newFinalDate).ToList();

                    _bd.Times.RemoveRange(recordsToDelete);

                    if (!Save()) { throw new UnstoredValuesException(); }

                }

                var recordToUpdate = _bd.PeriodsMedications.Find(periods.periodID);

                List<TimeOnly> timesPrevious = new List<TimeOnly>();

                var newRecordsToAList = GetDatesInRange(periods.finalFrec.AddDays(1), newFinalDate);

                var timesExamples = _bd.Times.Where(e => e.periodID == periods.periodID
                                                    && e.dateMedication == periods.finalFrec.AddDays(-1)).ToList();

                timesPrevious.AddRange(timesExamples.Select(e => e.time));

                //AddTimes(periods.periodID, medication.accountID, newRecordsToAList, timesPrevious);

                recordToUpdate.finalFrec = newFinalDate;

                _bd.PeriodsMedications.Update(recordToUpdate);

                if (!Save()) { throw new UnstoredValuesException(); }
            }

            foreach (var id in idsToRemove)
            {
                var existingRecords = _bd.Times.Any(e => e.periodID == id);

                if (!existingRecords)
                {
                    var recordToDelete = _bd.PeriodsMedications.Find(id);

                    _bd.PeriodsMedications.Remove(recordToDelete);

                    if (!Save()) { throw new UnstoredValuesException(); }

                }
            }
            
            return InfoMedicationJustAddUpdateDelete(medication.medicationID, periods.periodID, dateRecord);
        }

        private InfoMedicationDto UpdateForNewDailyFrec(UpdateMedicationUseDto values)
        {
            List<Guid> idsPrevious = new List<Guid>();
            List<Guid> ids = new List<Guid>();
            List<DateOnly> dates;

            Action<List<TimeListDto>, DateOnly> processRecords = (list, date) =>
            {
                foreach (var id in list)
                {
                    var record = _bd.Times.Find(id.timeID);

                    var recordsToUpdate = _bd.Times.Where(e => e.periodID == values.periodID
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

            if (!values.times.Any())
            {
                processRecords(values.timesPrevious, values.dateRecord);

                var recordsTimes = _bd.Times.Where(e => e.dateMedication == values.dateRecord
                                                  && e.periodID == values.periodID
                                                  /*&& e.accountID == values.accountID*/).ToList();

                idsPrevious.AddRange(recordsTimes.Select(e => e.timeID));

                foreach (var item in values.timesPrevious)
                {
                    ids.Add(item.timeID);
                }

                var findIdsToDelete = idsPrevious.Except(ids).ToList();

                foreach (var id in findIdsToDelete)
                {
                    var recordTime = _bd.Times.Find(id);

                    var recordsToDelete = _bd.Times.Where(e => e.periodID == values.periodID
                                                          && e.time == recordTime.time
                                                          && e.dateMedication >= values.dateRecord).ToList();

                    _bd.Times.RemoveRange(recordsToDelete);

                    if (!Save()) { throw new UnstoredValuesException(); }
                }

                return InfoMedicationJustAddUpdateDelete(values.medicationID, values.periodID, values.dateRecord);
            }
            else
            {
                processRecords(values.timesPrevious, values.dateRecord);

                var medication = _bd.Medications.Find(values.medicationID);

                dates = GetDatesInRange(values.dateRecord, values.finalFrec);

                //AddTimes(values.periodID, medication.accountID, dates, values.times);

                return InfoMedicationJustAddUpdateDelete(values.medicationID, values.periodID, values.dateRecord);
            }
        }
    }
}