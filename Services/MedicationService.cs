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

        public List<InfoMedicationDto> AddMedication(AddMedicationUseDto medication)
        {
            var medicationExisting = _bd.Medications.Any(e => e.accountID == medication.accountID 
                                        && e.nameMedication == medication.nameMedication
                                        && e.dose == medication.dose && e.initialFrec == medication.initialFrec
                                        && e.finalFrec == medication.finalFrec && e.dailyFrec == medication.dailyFrec);

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
                dailyFrec = medication.dailyFrec
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
                                        && e.finalFrec == medication.finalFrec && e.dailyFrec == medication.dailyFrec);

            AddTimes(recentlyMedication.medicationID, medication.accountID, dates, medication.times);

            var medicationsList = InfoMedicationJustAddUpdateDelete(medication.accountID, medication.dateRecord);

            return medicationsList;

        }

        public List<InfoMedicationDto> GetMedications(Guid id, DateOnly date)
        {
            DateOnly dateFinal = date.AddDays(-6);

            var recordsTimes = _bd.Times
                .Where(e => e.accountID == id && e.dateMedication >= dateFinal && e.dateMedication <= date)
                .ToList();

            List<InfoMedicationDto> listInfoMed = new List<InfoMedicationDto>();

            var listTimes = _mapper.Map<List<TimeListDto>>(recordsTimes);

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

                if (list.Count() > 0)
                {
                    InfoMedicationDto info = new InfoMedicationDto
                    {
                        medicationID = infoMed.medicationID,
                        accountID = infoMed.accountID,
                        nameMedication = infoMed.nameMedication,
                        dose = infoMed.dose,
                        initialFrec = infoMed.initialFrec,
                        finalFrec = infoMed.finalFrec,
                        dailyFrec = infoMed.dailyFrec,
                        times = list
                    };

                    listInfoMed.Add(info);
                }
            }

            listInfoMed = listInfoMed.OrderBy(x => x.times.Min(t => t.dateMedication)).ToList();

            return listInfoMed;
        }

        private List<InfoMedicationDto> InfoMedicationJustAddUpdateDelete(Guid id, DateOnly dateRecord)
        {
            List<InfoMedicationDto> listInfoMed = new List<InfoMedicationDto>();

            var recordsTimes = _bd.Times.Where(e => e.accountID == id && e.dateMedication == dateRecord).ToList();

            var listTimes = _mapper.Map<List<TimeListDto>>(recordsTimes);

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
                    dailyFrec = infoMed.dailyFrec,
                    times = list
                };

                listInfoMed.Add(info);
            }

            return listInfoMed;
        }

        public List<InfoMedicationDto> UpdateMedication(UpdateMedicationUseDto values)
        {
            var med = _bd.Medications.Find(values.medicationID);

            if (med == null)
            {
                throw new UnstoredValuesException();
            }

            if(med.initialFrec != values.initialFrec)
            {
                UpdateForNewDateInitial(med, values.initialFrec);
            }

            if(med.finalFrec != values.finalFrec)
            {
                UpdateForNewDateFinal(med, values.finalFrec);
            }

            if(med.dailyFrec != values.dailyFrec)
            {
                UpdateForNewDailyFrec(med, values);
            }

            Medication medication = new Medication
            {
                nameMedication = values.nameMedication,
                dose = values.dose,
                initialFrec = values.initialFrec,
                finalFrec = values.finalFrec,
                dailyFrec = values.dailyFrec
            };

            var valResults = new List<ValidationResult>();
            var valContext = new ValidationContext(medication, null, null);

            if (!Validator.TryValidateObject(medication, valContext, valResults, true))
            {
                var errors = valResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }

            _bd.Medications.Update(medication);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            var medicationsList = InfoMedicationJustAddUpdateDelete(values.accountID, values.dateRecord);

            return medicationsList;

        }

        public void UpdateStatusMedication(UpdateMedicationStatusDto value)
        {
            var record = _bd.Times.Find(value.timeID);

            record.medicationStatus = value.medicationStatus;

            _bd.Times.Update(record);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        public List<InfoMedicationDto> DeleteAMedication(Guid id, DateOnly date)
        {
            var recordInfoMedication = _bd.Medications.FirstOrDefault(e => e.medicationID == id);

            List<DateOnly> currentListDates = GetDatesInRange(recordInfoMedication.initialFrec, 
                                                recordInfoMedication.finalFrec);

            foreach (DateOnly dateOnly in currentListDates)
            {
                if(dateOnly == date)
                {
                    currentListDates.Remove(dateOnly);
                }
            }

            if(currentListDates.Count() == 1)
            {
                _bd.Medications.Remove(recordInfoMedication);
            }
            else
            {
                var recordsTimes = _bd.Times.Where(e => e.medicationID == id && e.dateMedication == date).ToList();

                _bd.Times.RemoveRange(recordsTimes);
            }

            if (!Save())
            {
                throw new UnstoredValuesException();
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

                    if (!Save())
                    {
                        throw new UnstoredValuesException();
                    }
                }
            }
        }

        private void UpdateForNewDateInitial(Medication medication, DateOnly newInitialDate)
        {
            List<Times> records;

            if(medication.finalFrec < newInitialDate)
            {
                throw new NewInitialDateAfterFinalDateException();
            }

            Action<Medication, DateOnly> processRecords = (medication, date) =>
            {
                Medication medicationOld = new Medication
                {
                    nameMedication = medication.nameMedication,
                    dose = medication.dose,
                    initialFrec = medication.initialFrec,
                    finalFrec = date,
                    dailyFrec = medication.dailyFrec
                };

                _bd.Medications.Add(medicationOld);

                var recentlyMedication = _bd.Medications.FirstOrDefault(e => e.accountID == medication.accountID
                                        && e.nameMedication == medication.nameMedication
                                        && e.dose == medication.dose && e.initialFrec == medication.initialFrec
                                        && e.finalFrec == date && e.dailyFrec == medication.dailyFrec);


                records = _bd.Times.Where(e => e.dateMedication <= date
                                    && e.medicationID == medication.medicationID).ToList();

                foreach (var item in records)
                {
                    item.medicationID = recentlyMedication.medicationID;
                    _bd.Times.Update(item);
                }

                medication.initialFrec = newInitialDate;

                _bd.Medications.Update(medication);


                if (!Save()) { throw new UnstoredValuesException(); }
            };

            processRecords(medication, newInitialDate);

            if (medication.finalFrec == newInitialDate)
            {
                DateOnly newDateFinal = medication.finalFrec.AddDays(-1);

                processRecords(medication, newDateFinal);
            }


            if (newInitialDate < medication.initialFrec)
            {
                var newRecordsToAList = GetDatesInRange(newInitialDate, medication.initialFrec);

                var timesExamples = _bd.Times.Where(e => e.medicationID == medication.medicationID).ToList();

                var groupByTimes = timesExamples.GroupBy(e => e.dateMedication)
                                   .ToDictionary(g => g.Key, g => g.Select(e => e.time).ToList());


                var times = groupByTimes.ContainsKey(medication.initialFrec)
                ? _mapper.Map<List<TimeOnly>>(groupByTimes[medication.initialFrec])
                : new List<TimeOnly>();

                AddTimes(medication.medicationID, medication.accountID, newRecordsToAList, times);

                medication.initialFrec = newInitialDate;

                _bd.Medications.Update(medication);

                if (!Save()) { throw new UnstoredValuesException(); }

            }
        }

        private void UpdateForNewDateFinal(Medication medication, DateOnly newFinalDate)
        {
            List<Times> records;

            if (newFinalDate < medication.initialFrec)
            {
                throw new NewFinalDateBeforeInitialDateException();
            }

            Action<Medication, DateOnly> processRecords = (medication, date) =>
            {
                DateOnly newDateInitial = date.AddDays(1);

                Medication medicationNew = new Medication
                {
                    nameMedication = medication.nameMedication,
                    dose = medication.dose,
                    initialFrec = newDateInitial,
                    finalFrec = medication.finalFrec,
                    dailyFrec = medication.dailyFrec
                };

                _bd.Medications.Add(medicationNew);

                var recentlyMedication = _bd.Medications.FirstOrDefault(e => e.accountID == medication.accountID
                                            && e.nameMedication == medication.nameMedication
                                            && e.dose == medication.dose && e.initialFrec == newDateInitial
                                            && e.finalFrec == medication.finalFrec && e.dailyFrec == medication.dailyFrec);

                records = _bd.Times.Where(e => e.dateMedication > newFinalDate
                                    && e.medicationID == medication.medicationID).ToList();

                foreach (var item in records)
                {
                    item.medicationID = recentlyMedication.medicationID;
                    _bd.Times.Update(item);
                }

                medication.finalFrec = date;
                _bd.Medications.Update(medication);

                if (!Save()) { throw new UnstoredValuesException(); }
            };

            processRecords(medication, newFinalDate);

            if(medication.finalFrec < newFinalDate)
            {
                var newRecordsToAList = GetDatesInRange(medication.finalFrec, newFinalDate);

                var timesExamples = _bd.Times.Where(e => e.medicationID == medication.medicationID).ToList();

                var groupByTimes = timesExamples.GroupBy(e => e.dateMedication)
                                   .ToDictionary(g => g.Key, g => g.Select(e => e.time).ToList());


                var times = groupByTimes.ContainsKey(medication.finalFrec)
                ? _mapper.Map<List<TimeOnly>>(groupByTimes[medication.finalFrec])
                : new List<TimeOnly>();

                AddTimes(medication.medicationID, medication.accountID, newRecordsToAList, times);

                medication.finalFrec = newFinalDate;
                _bd.Medications.Update(medication);

                if (!Save()) { throw new UnstoredValuesException(); }

            }
        }

        private void UpdateForNewDailyFrec(Medication medication, UpdateMedicationUseDto values)
        {
            List<Guid> IdsPrevious = new List<Guid>();
            List<Guid> Ids = new List<Guid>();

            //Si se añaden mas horarios a parte de los que ya tiene, es decir si la frecdia incrementa
            if (values.times.Any())
            {
                List<DateOnly> dates = GetDatesInRange(values.dateRecord, medication.finalFrec);

                AddTimes(medication.medicationID, medication.accountID, dates, values.times);
            }

            var recordsTimes = _bd.Times.Where(e => e.dateMedication == values.dateRecord
                                          && e.medicationID == values.medicationID
                                          && e.accountID == values.accountID);

            //Aqui actualiza los regitros que vienen en la lista que me mandaron
            foreach (var record in recordsTimes)
            {
                foreach (var time in values.timesPrevious)
                {
                    if (record.timeID.Equals(time.timeID))
                    {
                        record.time = time.time;

                        _bd.Times.Update(record);

                        if (!Save()) { throw new UnstoredValuesException(); }
                    }
                }
            }

            //Aqui eliminamos un horario si es que la frecDiaria disminuye
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
        
    }
}