using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Medication;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.Models.Medications;
using AppVidaSana.Services.IServices;
using AutoMapper;
using Microsoft.CodeAnalysis.CSharp;
using NuGet.Protocol;
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

        public List<InfoMedicationDto> AddMedication(AddMedicationUseDto medication)
        {
            var medicationExisting = _bd.Medications.Any(e => e.accountID == medication.accountID 
                                        && e.nameMedication == medication.nameMedication
                                        && e.dose == medication.dose && e.initialFrec == medication.initialFrec
                                        && e.finalFrec == medication.finalFrec);

            if (medicationExisting)
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
                dose = medication.dose,
                initialFrec = medication.initialFrec,
                finalFrec = medication.finalFrec,
                isActive = true
            };

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

            _bd.Medications.Add(med);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            List<DateOnly> dates = GetDatesInRange(medication.initialFrec, medication.finalFrec);

            var recentlyMedication = _bd.Medications.FirstOrDefault(e => e.accountID == medication.accountID
                                        && e.nameMedication == medication.nameMedication
                                        && e.dose == medication.dose && e.initialFrec == medication.initialFrec
                                        && e.finalFrec == medication.finalFrec);

            AddTimes(recentlyMedication.medicationID, medication.accountID, dates, medication.times);

            var medicationsList = InfoMedicationJustAddUpdateDelete(medication.accountID, medication.dateRecord);

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

            var groupObjectsByID = listTimes.GroupBy(obj => obj.medicationID)
                                    .ToDictionary(
                                        g => g.Key,
                                        g => g.ToList()
                                    );

            List<DateOnly> dates = GetDatesInRange(dateFinal, dateActual);

            foreach(var date in dates)
            {
                foreach (var time in groupObjectsByID)
                {
                    var med = _bd.Medications.Find(time.Key);

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

            var groupObjectsByIDActual = listTimesActual.GroupBy(obj => obj.medicationID)
                                    .ToDictionary(
                                        g => g.Key,
                                        g => g.ToList()
                                    );

            foreach (var time in groupObjectsByIDActual)
            {
                var medID = time.Key;
                var list = time.Value;

                var infoMed = _bd.Medications.Find(medID);

                if (list.Any())
                {
                    InfoMedicationDto infoMedication = new InfoMedicationDto
                    {
                        medicationID = infoMed.medicationID,
                        accountID = infoMed.accountID,
                        nameMedication = infoMed.nameMedication,
                        dose = infoMed.dose,
                        initialFrec = infoMed.initialFrec,
                        finalFrec = infoMed.finalFrec,
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

        public List<InfoMedicationDto> UpdateMedication(UpdateMedicationUseDto values)
        {
            var med = _bd.Medications.Find(values.medicationID);

            if (med == null)
            {
                throw new UnstoredValuesException();
            }
            
            UpdateForNewDailyFrec(med, values);
            
            if (med.initialFrec != values.initialFrec)
            {
                UpdateForNewDateInitial(med, values.initialFrec);
            }

            if (med.finalFrec != values.finalFrec)
            {
                UpdateForNewDateFinal(med, values.finalFrec);
            }

            med.nameMedication = values.nameMedication;
            med.dose = values.dose;
            med.initialFrec = values.initialFrec;
            med.finalFrec = values.finalFrec;

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

            _bd.Medications.Update(med);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            var medicationsList = InfoMedicationJustAddUpdateDelete(values.accountID, values.dateRecord);

            return medicationsList;

        }

        public List<InfoMedicationDto> DeleteAMedication(Guid id, DateOnly date)
        {
            Action<Guid, DateOnly> processRecords = (medicationID, dateRecord) =>
            {
                var records = _bd.Times.Where(e => e.medicationID == medicationID && e.dateMedication == dateRecord).ToList();
               
                _bd.Times.RemoveRange(records);

                if (!Save()) { throw new UnstoredValuesException(); }
            };

            var recordInfoMedication = _bd.Medications.FirstOrDefault(e => e.medicationID == id);

            if(recordInfoMedication.initialFrec == date)
            {
                recordInfoMedication.initialFrec = recordInfoMedication.initialFrec.AddDays(1);

                _bd.Medications.Update(recordInfoMedication);

                if (!Save()) { throw new UnstoredValuesException(); }

                processRecords(id, date);
            }

            if (recordInfoMedication.finalFrec == date)
            {
                recordInfoMedication.finalFrec = recordInfoMedication.finalFrec.AddDays(-1);

                _bd.Medications.Update(recordInfoMedication);

                if (!Save()) { throw new UnstoredValuesException(); }

                processRecords(id, date);
            }

            if(recordInfoMedication.initialFrec == date && recordInfoMedication.finalFrec == date)
            {
                processRecords(id, date);

                _bd.Medications.Remove(recordInfoMedication);

                if (!Save()) { throw new UnstoredValuesException(); }
            }
            else
            {
                processRecords(id, date);
            }

            var medicationsList = InfoMedicationJustAddUpdateDelete(id, date);

            return medicationsList;
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

        private List<InfoMedicationDto> InfoMedicationJustAddUpdateDelete(Guid id, DateOnly dateRecord)
        {
            List<InfoMedicationDto> listInfoMed = new List<InfoMedicationDto>();

            var recordsTimes = _bd.Times.Where(e => e.accountID == id && e.dateMedication == dateRecord).ToList();

            var listTimes = _mapper.Map<List<TimeListDto>>(recordsTimes);

            listTimes = listTimes.OrderBy(x => x.time).ToList();

            var groupObjectsByID = listTimes.GroupBy(obj => obj.medicationID)
                                    .ToDictionary(
                                        g => g.Key,
                                        g => g.ToList()
                                    );

            foreach (var time in groupObjectsByID)
            {
                var medID = time.Key;
                var list = time.Value;

                var infoMed = _bd.Medications.Find(medID);

                InfoMedicationDto info = new InfoMedicationDto
                {
                    medicationID = infoMed.medicationID,
                    accountID = infoMed.accountID,
                    nameMedication = infoMed.nameMedication,
                    dose = infoMed.dose,
                    initialFrec = infoMed.initialFrec,
                    finalFrec = infoMed.finalFrec,
                    times = list
                };

                listInfoMed.Add(info);
            }

            return listInfoMed;
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

        private void AddTimes(Guid medicamentID, Guid accountID, List<DateOnly> dates, List<TimeOnly> times)
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
                        accountID = accountID,
                        medicationID = medicamentID,
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

        public void UpdateStatusMedication(UpdateMedicationStatusDto value)
        {
            var record = _bd.Times.Find(value.timeID);

            record.medicationStatus = value.medicationStatus;

            _bd.Times.Update(record);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private void UpdateForNewDateInitial(Medication medication, DateOnly newInitialDate)
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
        }

        private void UpdateForNewDailyFrec(Medication medication, UpdateMedicationUseDto values)
        {
            List<Guid> IdsPrevious = new List<Guid>();
            List<Guid> Ids = new List<Guid>();
            List<DateOnly> dates;

            var verifingIsActive = _bd.Medications.Find(medication.medicationID);

            if (!verifingIsActive.isActive)
            {
                throw new NotEditingException();
            }

            Action<List<TimeListDto>, DateOnly> processRecords = (list, date) =>
            {
                foreach (var id in list)
                {
                    var record = _bd.Times.Find(id.timeID);

                    var recordsToUpdate = _bd.Times.Where(e => e.medicationID == medication.medicationID
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
                                          && e.medicationID == values.medicationID
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

                    var recordsToDelete = _bd.Times.Where(e => e.medicationID == medication.medicationID
                                                        && e.time == recordTime.time
                                                        && e.dateMedication >= values.dateRecord).ToList();

                    _bd.Times.RemoveRange(recordsToDelete);
 
                    if (!Save()) { throw new UnstoredValuesException(); }

                }
            }
            else
            {
                processRecords(values.timesPrevious, values.dateRecord);

                dates = GetDatesInRange(values.dateRecord, medication.finalFrec);

                AddTimes(medication.medicationID, medication.accountID, dates, values.times);
            }

        }
        
    }
}