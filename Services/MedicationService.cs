using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Medication;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.Models.Medications;
using AppVidaSana.Services.IServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

        public List<InfoMedicationDto> UpdateMedication(UpdateMedicationUseDto values)
        {
            var med = _bd.Medications.Find(values.medicationID);

            if (med == null)
            {
                throw new UnstoredValuesException();
            }

            if(med.initialFrec != values.initialFrec)
            {
                UpdateForNewDateInitial(med, values.dateRecord, values.initialFrec);
            }

            if(med.finalFrec != values.finalFrec)
            {
                UpdateForNewDateFinal(med, values.dateRecord, values.finalFrec);
            }

            if(med.dailyFrec != values.dailyFrec)
            {
                UpdateForNewDailyFrec(med, values.times, values.dateRecord, values.dailyFrec);
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

        public List<InfoMedicationDto> DeleteAMedication(Guid id, DateOnly date)
        {
            var recordInfoMedication = _bd.Medications.FirstOrDefault(e => e.medicationID == id);

            List<DateOnly> currentListDates = GetDatesInRange(recordInfoMedication.initialFrec, recordInfoMedication.finalFrec);

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

        public static List<DateOnly> GetDatesInRange(DateOnly startDate, DateOnly endDate)
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

        public List<InfoMedicationDto> InfoMedicationJustAddUpdateDelete(Guid id, DateOnly dateRecord)
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

            return listInfoMed;
        }

        public void AddTimes(Guid medicamentID, Guid accountID, List<DateOnly>dates, List<TimeOnly> times)
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

        public void UpdateForNewDateInitial(Medication medication, DateOnly dateUpdate, DateOnly newInitialDate)
        {
            List<Times> records1, records2, records3;
            List<DateOnly> list1, list2, list3;

            if(medication.finalFrec < newInitialDate)
            {
                throw new UnstoredValuesException();
            }

            Func<AppDbContext, Guid , DateOnly, DateOnly, List<Times>> compiledQuery = EF.CompileQuery(
                        (AppDbContext context, Guid id, DateOnly date1, DateOnly date2) =>
                           context.Times.Where(e => date1 <= e.dateMedication && e.dateMedication <= date2
                                                && e.medicationID == id).ToList()
                        );

            Action<List<Times>, List<DateOnly>> processRecords = (records, list) =>
            {
                var groupByDates = records.GroupBy(e => e.dateMedication).ToDictionary(
                                g => g.Key,
                                g => g.Select(e => e.medicationStatus).ToList());

                foreach (var date in list)
                {
                    int countStatusConsumed = 0;

                    var times = groupByDates.ContainsKey(date);

                    if (times)
                    {
                        foreach (var status in groupByDates[date])
                        {

                            countStatusConsumed = (status == true) ? ++countStatusConsumed : 0;
                        }

                        if (countStatusConsumed == 0)
                        {
                            var recordsToDelete = _bd.Times.Where(e => e.medicationID == medication.medicationID
                                                        && e.dateMedication == date).ToList();

                            _bd.Times.RemoveRange(recordsToDelete);

                            if (!Save()) { throw new UnstoredValuesException(); }
                        }
                    }
                }
            };


            if (medication.initialFrec <= newInitialDate && newInitialDate <= dateUpdate) {
                
                //initialFrecPrevious <= newInitialDate
                records1 = _bd.Times.Where(e => e.dateMedication <= newInitialDate 
                                    && e.medicationID == medication.medicationID).ToList();

                list1 = GetDatesInRange(medication.initialFrec, newInitialDate);

                processRecords(records1, list1);

                //newInitialDate <= dateUpdate
                records2 = compiledQuery(_bd, medication.medicationID, newInitialDate, dateUpdate);

                list2 = GetDatesInRange(newInitialDate, dateUpdate);

                processRecords(records2, list2);

            }

            if (dateUpdate <= newInitialDate)
            {
                //dateUpdate <= newInitialDate
                records3 = compiledQuery(_bd, medication.medicationID, dateUpdate, newInitialDate);

                list3 = GetDatesInRange(newInitialDate, dateUpdate);

                processRecords(records3, list3);

            }


            if (newInitialDate < medication.initialFrec)
            {
                //newFinalDate <= initialFrecPrevious
                var newRecordsToAList = GetDatesInRange(newInitialDate, medication.initialFrec);

                var timesExamples = _bd.Times.Where(e => e.medicationID == medication.medicationID).ToList();

                var groupByTimes = timesExamples.GroupBy(e => e.dateMedication)
                                   .ToDictionary(g => g.Key, g => g.Select(e => e.time).ToList());


                var times = groupByTimes.ContainsKey(medication.initialFrec)
                ? _mapper.Map<List<TimeOnly>>(groupByTimes[medication.initialFrec])
                : new List<TimeOnly>();

                AddTimes(medication.medicationID, medication.accountID, newRecordsToAList, times);

            }
        }

        public void UpdateForNewDateFinal(Medication medication, DateOnly dateUpdate, DateOnly newFinalDate)
        {
            List<Times> records1, records2, records3, records4;
            List<DateOnly> list1, list2, list3, list4;

            if(newFinalDate < medication.initialFrec)
            {
                throw new UnstoredValuesException();
            }

            Func<AppDbContext, Guid , DateOnly, DateOnly, List<Times>> compiledQuery = EF.CompileQuery(
                        (AppDbContext context, Guid id, DateOnly date1, DateOnly date2) =>
                           context.Times.Where(e => date1 <= e.dateMedication && e.dateMedication <= date2
                                                && e.medicationID == id).ToList());

            Action<List<Times>, List<DateOnly>> processRecords = (records, list) =>
            {
                var groupByDates = records.GroupBy(e => e.dateMedication).ToDictionary(
                                g => g.Key,
                                g => g.Select(e => e.medicationStatus).ToList());

                foreach (var date in list)
                {
                    int countStatusConsumed = 0;

                    var times = groupByDates.ContainsKey(date);

                    if (times)
                    {
                        foreach (var status in groupByDates[date])
                        {

                            countStatusConsumed = (status == true) ? ++countStatusConsumed : 0;
                        }

                        if (countStatusConsumed == 0)
                        {
                            var recordsToDelete = _bd.Times.Where(e => e.medicationID == medication.medicationID
                                                        && e.dateMedication == date).ToList();

                            _bd.Times.RemoveRange(recordsToDelete);

                            if (!Save()) { throw new UnstoredValuesException(); }
                        }
                    }
                }
            };
                        
            if (dateUpdate <= newFinalDate && newFinalDate <= medication.finalFrec)
            {
                //dateUpdate <= newFinalDate
                list1 = GetDatesInRange(dateUpdate, newFinalDate);

                records1 = compiledQuery(_bd, medication.medicationID, dateUpdate, newFinalDate);

                processRecords(records1, list1);

                //newFinalDate <= finalFrecPrevious
                records2 = compiledQuery(_bd, medication.medicationID, newFinalDate, medication.finalFrec);

                list2 = GetDatesInRange(newFinalDate, dateUpdate);

                processRecords(records2, list2);

            }

            if(medication.initialFrec <= newFinalDate && newFinalDate <= dateUpdate)
            {
                //initialFrecPrevious <= newFinalDate
                records3 = compiledQuery(_bd, medication.medicationID, medication.initialFrec, newFinalDate);

                list3 = GetDatesInRange(dateUpdate, newFinalDate);

                processRecords(records3, list3);

                //newFinalDate <= dateUpdate
                records4 = compiledQuery(_bd, medication.medicationID, newFinalDate, dateUpdate);

                list4 = GetDatesInRange(newFinalDate, dateUpdate);

                processRecords(records4, list4);

            }

            if(medication.finalFrec < newFinalDate)
            {
                //finalFrecPrevious <= newFinalDate
                var newRecordsToAList = GetDatesInRange(medication.finalFrec, newFinalDate);

                var timesExamples = _bd.Times.Where(e => e.medicationID == medication.medicationID).ToList();

                var groupByTimes = timesExamples.GroupBy(e => e.dateMedication)
                                   .ToDictionary(g => g.Key, g => g.Select(e => e.time).ToList());


                var times = groupByTimes.ContainsKey(medication.finalFrec)
                ? _mapper.Map<List<TimeOnly>>(groupByTimes[medication.finalFrec])
                : new List<TimeOnly>();

                AddTimes(medication.medicationID, medication.accountID, newRecordsToAList, times);

            }
        }

        public void UpdateForNewDailyFrec(Medication medication, List<TimeOnly> times, DateOnly dateUpdate, int newDailyFrec)
        {
            List<DateOnly> newRecords = new List<DateOnly>();
            List<DateOnly> beforeDates, afterDates;

            Action<List<Times>, List<DateOnly>> processRecords = (records, list) =>
            {
                var groupByDates = records.GroupBy(e => e.dateMedication).ToDictionary(
                                g => g.Key,
                                g => g.Select(e => e.medicationStatus).ToList());

                foreach (var date in list)
                {
                    int countStatusConsumed = 0;

                    var status = groupByDates.ContainsKey(date);

                    if (status)
                    {
                        foreach (var s in groupByDates[date])
                        {

                            countStatusConsumed = (s == true) ? ++countStatusConsumed : 0;
                        }

                        if (countStatusConsumed == 0)
                        {
                            var recordsToDelete = _bd.Times.Where(e => e.medicationID == medication.medicationID
                                                        && e.dateMedication == date).ToList();

                            _bd.Times.RemoveRange(recordsToDelete);

                            if (!Save()) { throw new UnstoredValuesException(); }


                            newRecords.Add(date);
                        }
                    }
                }
            };


            var recordsBeforeDateUpdate = _bd.Times.Where(e => e.dateMedication <= dateUpdate).ToList();
            beforeDates = GetDatesInRange(medication.initialFrec, dateUpdate);
            processRecords(recordsBeforeDateUpdate, beforeDates);
            
            var recordsAfterDateUpdate = _bd.Times.Where(e => dateUpdate <= e.dateMedication).ToList();
            afterDates = GetDatesInRange(dateUpdate, medication.finalFrec);
            processRecords(recordsAfterDateUpdate, afterDates);
  

            if (newRecords.Count() > 0)
            {
                AddTimes(medication.medicationID, medication.accountID, newRecords, times);
            }
        }
    }
}