using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Medication;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.Models.Medications;
using AppVidaSana.Services.IServices;
using AutoMapper;
using NuGet.Packaging.Signing;
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
            var medicationExisting = _bd.Medications.Any(e => e.accountID == medication.accountID 
                                                        && e.nameMedication == medication.nameMedication
                                                        && e.dose == medication.dose);

            var periodExisting = _bd.PeriodsMedications.Any(e => e.initialFrec == medication.initialFrec
                                                            && e.finalFrec == medication.finalFrec);

            if (medicationExisting && periodExisting)
            {
                throw new RepeatRegistrationException();
            }

            var user = _bd.Accounts.Find(medication.accountID);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            Medication med = new Medication
            {
                accountID = medication.accountID,
                nameMedication = medication.nameMedication,
                dose = medication.dose
            };

            ValidationAddMedication(med);

            _bd.Medications.Add(med);

            if (!Save()) { throw new UnstoredValuesException(); }


            Guid medID = _bd.Medications.FirstOrDefault(e => e.accountID == medication.accountID
                                                            && e.nameMedication == medication.nameMedication
                                                            && e.dose == medication.dose).medicationID;

            PeriodsMedications periods = new PeriodsMedications
            {
                medicationID = medID,
                initialFrec = medication.initialFrec,
                finalFrec = medication.finalFrec,
                isActive = true
            };

            ValidationAddPeriodsMedications(periods);

            _bd.PeriodsMedications.Add(periods);

            if (!Save()) { throw new UnstoredValuesException(); }


            List<DateOnly> dates = GetDatesInRange(medication.initialFrec, medication.finalFrec);

            var recentlyPeriod = _bd.PeriodsMedications.FirstOrDefault(e => e.medicationID == medID
                                                                    && e.initialFrec == medication.initialFrec
                                                                    && e.finalFrec == medication.finalFrec);

            AddTimes(recentlyPeriod.periodID, medication.accountID, dates, medication.times);

            var medicationsList = InfoMedicationJustAddUpdateDelete(recentlyPeriod.periodID, medication.dateRecord);

            return medicationsList;

        }

        public MedicationsAndValuesGraphicDto GetMedications(Guid id, DateOnly dateActual)
        {
            int countStatus = 0, totalMedications = 0;
            bool statusGeneral = false;
            List<MedicationDigestDto> listMedications = new List<MedicationDigestDto>();
            List<WeeklyAttachmentDto> weeklyList = new List<WeeklyAttachmentDto>();

            DateOnly dateFinal = dateActual.AddDays(-6);

            var recordsTimes = _bd.Times
                .Where(e => e.accountID == id && e.dateMedication >= dateFinal && e.dateMedication <= dateActual)
                .ToList();

            List<InfoMedicationDto> listInfoMed = new List<InfoMedicationDto>();

            var listTimes = _mapper.Map<List<TimeListDto>>(recordsTimes);

            listTimes = listTimes.OrderBy(x => x.time).ToList();

            var groupObjectsByID = listTimes.GroupBy(obj => obj.periodID)
                                    .ToDictionary(
                                        g => g.Key,
                                        g => g.ToList()
                                    );

            List<DateOnly> dates = GetDatesInRange(dateFinal, dateActual);

            foreach(var date in dates)
            {
                foreach (var time in groupObjectsByID)
                {
                    var period = _bd.PeriodsMedications.Find(time.Key);

                    var med = _bd.Medications.Find(period.medicationID);

                    var list = time.Value.Where(e => e.dateMedication == date).ToList();

                    if (list.Any())
                    {
                        totalMedications++;

                        foreach (var l in list)
                        {
                            if (l.medicationStatus){ countStatus++; }
                        }

                        statusGeneral = (countStatus == list.Count()) ? true : statusGeneral;

                        if (statusGeneral)
                        {
                            --totalMedications;
                        }

                        MedicationDigestDto medDigest = new MedicationDigestDto
                        {
                            medicationID = time.Key,
                            statusGeneral = statusGeneral
                        };

                        listMedications.Add(medDigest);
                    }
                }

                WeeklyAttachmentDto obj = new WeeklyAttachmentDto
                {
                    date = date,
                    totalMedications = totalMedications,
                    listMedications = listMedications
                };

                weeklyList.Add(obj);

                totalMedications = 0;
                listMedications = new List<MedicationDigestDto>();
            }

            var recordsTimesActual = _bd.Times
                .Where(e => e.accountID == id && e.dateMedication == dateActual)
                .ToList();

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

                var infoMed = _bd.Medications.Find(period.medicationID);

                if (list.Any())
                {
                    InfoMedicationDto infoMedication = new InfoMedicationDto
                    {
                        medicationID = infoMed.medicationID,
                        accountID = infoMed.accountID,
                        nameMedication = infoMed.nameMedication,
                        dose = infoMed.dose,
                        initialFrec = period.initialFrec,
                        finalFrec = period.finalFrec,
                        times = list
                    };

                    listInfoMed.Add(infoMedication);
                }
            }

            MedicationsAndValuesGraphicDto info = new MedicationsAndValuesGraphicDto
            {
                medications = listInfoMed,
                weeklyAttachments = weeklyList
            };

            return info;
        }

        public InfoMedicationDto UpdateMedication(UpdateMedicationUseDto values)
        {
            InfoMedicationDto info = new InfoMedicationDto();

            var med = _bd.Medications.Find(values.medicationID);

            if (med == null)
            {
                throw new UnstoredValuesException();
            }

            var period = _bd.PeriodsMedications.FirstOrDefault(e => e.medicationID == med.medicationID
                                                               && e.isActive == true);

            if (period == null)
            {
                throw new NotEditingException();
            }

            UpdateForNewDailyFrec(med, period, values);
            
            if (period.initialFrec != values.initialFrec || period.finalFrec != values.finalFrec)
            {
                info =  UpdateForNewDateInitialAndFinal(med, period, values.dateRecord, values.initialFrec, values.finalFrec);
            }

            med.nameMedication = values.nameMedication;
            med.dose = values.dose;

            ValidationAddMedication(med);

            _bd.Medications.Update(med);

            if (!Save()) { throw new UnstoredValuesException(); }

            return info;

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

            var updateInitialFrec = _bd.PeriodsMedications.Any(e => e.medicationID == id
                                                               && e.initialFrec == date);

            var updateFinalFrec = _bd.PeriodsMedications.Any(e => e.medicationID == id
                                                               && e.finalFrec == date);

            var lastRecord = _bd.PeriodsMedications.Any(e => e.medicationID == id
                                                        && e.initialFrec == date && e.finalFrec == date);

            if (updateInitialFrec)
            {
                var recordToUpdateInitialFrec = _bd.PeriodsMedications.FirstOrDefault(e => e.medicationID == id
                                                                                      && e.initialFrec == date);

                recordToUpdateInitialFrec.initialFrec = recordToUpdateInitialFrec.initialFrec.AddDays(1);

                _bd.PeriodsMedications.Update(recordToUpdateInitialFrec);

                if (!Save()) { throw new UnstoredValuesException(); }

                processRecords(recordToUpdateInitialFrec.periodID, date);
            }

            if (updateFinalFrec)
            {
                var recordToUpdateFinalFrec = _bd.PeriodsMedications.FirstOrDefault(e => e.medicationID == id
                                                                                      && e.finalFrec == date);

                recordToUpdateFinalFrec.finalFrec = recordToUpdateFinalFrec.finalFrec.AddDays(-1);

                _bd.PeriodsMedications.Update(recordToUpdateFinalFrec);

                if (!Save()) { throw new UnstoredValuesException(); }

                processRecords(recordToUpdateFinalFrec.periodID, date);
            }

            if(lastRecord)
            {
                var record = _bd.PeriodsMedications.FirstOrDefault(e => e.medicationID == id
                                                        && e.initialFrec == date && e.finalFrec == date);

                processRecords(record.periodID, date);

                var recordsToDelete = _bd.Times.Where(e => e.periodID == record.periodID).ToList();

                _bd.Times.RemoveRange(recordsToDelete);

                if (!Save()) { throw new UnstoredValuesException(); }

                _bd.Medications.Remove(_bd.Medications.Find(record.medicationID));

                if (!Save()) { throw new UnstoredValuesException(); }
            }
            else
            {
                var record = _bd.PeriodsMedications.FirstOrDefault(e => e.medicationID == id
                                                        && e.initialFrec < date && date <  e.finalFrec);

                processRecords(record.periodID, date);
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

        private InfoMedicationDto InfoMedicationJustAddUpdateDelete(Guid periodID, DateOnly dateRecord)
        {
            var recordsTimes = _bd.Times.Where(e => e.periodID == periodID
                                               && e.dateMedication == dateRecord).ToList();

            var listTimes = _mapper.Map<List<TimeListDto>>(recordsTimes);

            listTimes = listTimes.OrderBy(x => x.time).ToList();

            var period = _bd.PeriodsMedications.Find(periodID);

            var med = _bd.Medications.Find(period.medicationID);

            InfoMedicationDto infoMed = new InfoMedicationDto
            {
                medicationID = period.medicationID,
                accountID = med.accountID,
                nameMedication = med.nameMedication,
                dose = med.dose,
                initialFrec = period.initialFrec,
                finalFrec = period.finalFrec,
                times = listTimes
            };

            return infoMed;
        }

        private void ValidationAddMedication(Medication med)
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

        private void ValidationAddPeriodsMedications(PeriodsMedications periods)
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
                        accountID = accountID,
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
                                                                  DateOnly dateRecord, DateOnly newInitialDate, DateOnly newFinalDate)
        {
            var idsPeriodsInactive = _bd.PeriodsMedications.Where(e => e.medicationID == medication.medicationID
                                                                  && e.isActive == false).ToList();

            List<Guid> idsToRemove = new List<Guid>();

            idsToRemove.AddRange(idsPeriodsInactive.Select(e => e.periodID));

            foreach(var id in idsToRemove)
            {
                var existingRecords = _bd.Times.Any(e => e.periodID == id);

                if (!existingRecords)
                {
                    var recordToDelete = _bd.PeriodsMedications.Find(id);

                    _bd.PeriodsMedications.Remove(recordToDelete);

                    if (!Save()) { throw new UnstoredValuesException(); }

                }
            }

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
                List<TimeOnly> timesPrevious = new List<TimeOnly>();
                List<Guid> idsToPeriodsDelete = new List<Guid>();

                var recordsExamples = _bd.Times.Where(e => e.periodID == periods.periodID
                                                    && e.dateMedication == dateRecord).ToList();

                timesPrevious.AddRange(recordsExamples.Select(e => e.time));

                var deleteRecords = _bd.PeriodsMedications.Where(e => e.medicationID == medication.medicationID).ToList();
                
                foreach(var obj in deleteRecords)
                {
                    var records = _bd.Times.Where(e => e.periodID == obj.periodID).ToList();

                    _bd.Times.RemoveRange(records);

                    if (!Save()) { throw new UnstoredValuesException(); }
                }

                var lastRecords = _bd.PeriodsMedications.Where(e => e.medicationID == periods.medicationID
                                                            && e.initialFrec == newInitialDate
                                                            && e.finalFrec == newFinalDate && e.isActive == false).ToList();
                
                List<Guid> idToReturn = new List<Guid>();

                idToReturn.AddRange(lastRecords.Select(e => e.periodID));

                idsToPeriodsDelete.AddRange(deleteRecords.Select(e => e.periodID));

                var ids = idsToPeriodsDelete.Except(idToReturn).ToList();

                processRecords(ids);

                foreach (var item in lastRecords)
                {
                    item.isActive = true;

                    _bd.PeriodsMedications.Update(item);

                    if (!Save()) { throw new UnstoredValuesException(); }

                    List<DateOnly> dates = GetDatesInRange(newInitialDate, newFinalDate);

                    AddTimes(item.periodID, medication.accountID, dates, timesPrevious);

                    return InfoMedicationJustAddUpdateDelete(item.periodID, dateRecord);
                }
            }

            if (periods.initialFrec < newInitialDate && newFinalDate < periods.finalFrec)
            {
                periods.isActive = false;

                ValidationAddPeriodsMedications(periods);

                _bd.PeriodsMedications.Update(periods);

                if (!Save()) { throw new UnstoredValuesException(); }

                PeriodsMedications newPeriod = new PeriodsMedications
                {
                    medicationID = medication.medicationID,
                    initialFrec = newInitialDate,
                    finalFrec = newFinalDate,
                    isActive = true
                };

                ValidationAddPeriodsMedications(newPeriod);

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

                return InfoMedicationJustAddUpdateDelete(recentlyPeriodMedication.periodID, dateRecord);
            }

            if(periods.initialFrec == newInitialDate && newFinalDate < periods.finalFrec || periods.initialFrec < newInitialDate && newFinalDate == periods.finalFrec)
            {
                periods.isActive = false;

                ValidationAddPeriodsMedications(periods);

                _bd.PeriodsMedications.Update(periods);

                if (!Save()) { throw new UnstoredValuesException(); }

                PeriodsMedications newPeriod = new PeriodsMedications
                {
                    medicationID = medication.medicationID,
                    initialFrec = newInitialDate,
                    finalFrec = newFinalDate,
                    isActive = true
                };

                ValidationAddPeriodsMedications(newPeriod);

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

                return InfoMedicationJustAddUpdateDelete(recentlyPeriodMedication.periodID, dateRecord);
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

                AddTimes(periods.periodID, medication.accountID, newRecordsToAList, timesPrevious);

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

                AddTimes(periods.periodID, medication.accountID, newRecordsToAList, timesPrevious);

                recordToUpdate.finalFrec = newFinalDate;

                _bd.PeriodsMedications.Update(recordToUpdate);

                if (!Save()) { throw new UnstoredValuesException(); }
            }

            return InfoMedicationJustAddUpdateDelete(periods.periodID, dateRecord); ;
        }

        private void UpdateForNewDailyFrec(Medication medication, PeriodsMedications periods, UpdateMedicationUseDto values)
        {
            List<Guid> IdsPrevious = new List<Guid>();
            List<Guid> Ids = new List<Guid>();
            List<DateOnly> dates;

            Action<List<TimeListDto>, DateOnly> processRecords = (list, date) =>
            {
                foreach (var id in list)
                {
                    var record = _bd.Times.Find(id.timeID);

                    var recordsToUpdate = _bd.Times.Where(e => e.periodID == periods.periodID
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
                                          && e.periodID == periods.periodID
                                          && e.accountID == values.accountID).ToList();

                IdsPrevious.AddRange(recordsTimes.Select(e => e.timeID));

                foreach (var item in values.timesPrevious)
                {
                    Ids.Add(item.timeID);
                }

                var missingIds = IdsPrevious.Except(Ids).ToList();

                foreach (var id in missingIds)
                {
                    var recordTime = _bd.Times.Find(id);

                    var recordsToDelete = _bd.Times.Where(e => e.periodID == periods.periodID
                                                        && e.time == recordTime.time
                                                        && e.dateMedication >= values.dateRecord).ToList();

                    _bd.Times.RemoveRange(recordsToDelete);

                    if (!Save()) { throw new UnstoredValuesException(); }

                }
            }
            else
            {
                processRecords(values.timesPrevious, values.dateRecord);

                dates = GetDatesInRange(values.dateRecord, periods.finalFrec);

                AddTimes(periods.periodID, medication.accountID, dates, values.times);
            }

        }

        /*private void UpdateForNewDateInitial(Medication medication, DateOnly newInitialDate)
        {
            List<Times> records,recordsSample;
            List<DateOnly> newInitialDateInTime = new List<DateOnly>();
            List<TimeOnly> addTimes = new List<TimeOnly>();

            var verifingIsActive = _bd.Medications.Find(medication.medicationID);

            if (!verifingIsActive.isActive)
            {
                throw new NotEditingException();
            }

            if (medication.finalFrec < newInitialDate)
            {
                throw new NewInitialDateAfterFinalDateException();
            }

            Action<Medication, DateOnly> processRecords = (medication, date) =>
            {
                Medication medicationOld = new Medication
                {
                    accountID = medication.accountID,
                    nameMedication = medication.nameMedication,
                    dose = medication.dose,
                    initialFrec = medication.initialFrec,
                    finalFrec = date.AddDays(-1),
                    isActive = false
                };

                _bd.Medications.Add(medicationOld);

                if (!Save()) { throw new UnstoredValuesException(); }

                var recentlyMedication = _bd.Medications.FirstOrDefault(e => e.accountID == medicationOld.accountID
                                        && e.nameMedication == medicationOld.nameMedication
                                        && e.dose == medicationOld.dose && e.initialFrec == medicationOld.initialFrec
                                        && e.finalFrec == date.AddDays(-1));

                records = _bd.Times.Where(e => e.dateMedication < date
                                    && e.medicationID == medication.medicationID).ToList();

                foreach (var item in records)
                {
                    item.medicationID = recentlyMedication.medicationID;
                }

                _bd.Times.UpdateRange(records);

                medication.initialFrec = date;

                _bd.Medications.Update(medication);

                if (!Save()) { throw new UnstoredValuesException(); }

            };

            if (newInitialDate < medication.initialFrec)
            {
                List<TimeOnly> timesPrevious = new List<TimeOnly>();

                var newRecordsToAList = GetDatesInRange(newInitialDate, medication.initialFrec.AddDays(-1));

                var recordsExamples = _bd.Times.Where(e => e.medicationID == medication.medicationID
                                                    && e.dateMedication == medication.initialFrec).ToList();

                timesPrevious.AddRange(recordsExamples.Select(e => e.time));

                AddTimes(medication.medicationID, medication.accountID, newRecordsToAList, timesPrevious);

                medication.initialFrec = newInitialDate;

                _bd.Medications.Update(medication);

                if (!Save()) { throw new UnstoredValuesException(); }

            }
            else
            {
                processRecords(medication, newInitialDate);
            }
        }

        private void UpdateForNewDateFinal(Medication medication, DateOnly newFinalDate)
        {
            if (newFinalDate < medication.initialFrec)
            {
                throw new NewFinalDateBeforeInitialDateException();
            }

            var verifingIsActive = _bd.Medications.Find(medication.medicationID);

            if (!verifingIsActive.isActive)
            {
                throw new NotEditingException();
            }

            Action<Medication, DateOnly> processRecords = (medication, date) =>
            {
                DateOnly newDateInitial = date.AddDays(1);

                Medication medicationActive = new Medication
                {
                    accountID = medication.accountID,
                    nameMedication = medication.nameMedication,
                    dose = medication.dose,
                    initialFrec = newDateInitial,
                    finalFrec = medication.finalFrec,
                    isActive = false
                };

                _bd.Medications.Add(medicationActive);

                if (!Save()) { throw new UnstoredValuesException(); }

                var recentlyMedication = _bd.Medications.FirstOrDefault(e => e.accountID == medicationActive.accountID
                                            && e.nameMedication == medicationActive.nameMedication
                                            && e.dose == medicationActive.dose && e.initialFrec == newDateInitial
                                            && e.finalFrec == medicationActive.finalFrec);

                var records = _bd.Times.Where(e => date < e.dateMedication
                                    && e.medicationID == medication.medicationID).ToList();

                foreach (var item in records)
                {
                    item.medicationID = recentlyMedication.medicationID;
                }

                _bd.Times.UpdateRange(records);

                medication.finalFrec = date;

                _bd.Medications.Update(medication);

                if (!Save()) { throw new UnstoredValuesException(); }
            };

            if(medication.finalFrec < newFinalDate)
            {
                List<TimeOnly> timesPrevious = new List<TimeOnly>();

                var newRecordsToAList = GetDatesInRange(medication.finalFrec.AddDays(1), newFinalDate);

                var timesExamples = _bd.Times.Where(e => e.medicationID == medication.medicationID
                                                    && e.dateMedication == medication.finalFrec).ToList();

                timesPrevious.AddRange(timesExamples.Select(e => e.time));

                AddTimes(medication.medicationID, medication.accountID, newRecordsToAList, timesPrevious);

                medication.finalFrec = newFinalDate;

                _bd.Medications.Update(medication);

                if (!Save()) { throw new UnstoredValuesException(); }

            }
            else
            {
                processRecords(medication, newFinalDate);
            }
        }*/

    }
}