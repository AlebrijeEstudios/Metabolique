using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Ejercicio;
using AppVidaSana.Exceptions.Medication;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.Models.Medications;
using AppVidaSana.Services.IServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;
using NuGet.Packaging.Signing;
using Sprache;
using System.ComponentModel.DataAnnotations;
using Xunit;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            var medicationExisting = _bd.Medications.Any(e => e.dateRecord == medication.dateRecord
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

            List<DateOnly> dates = GetDatesInRange(medication.initialFrec, medication.finalFrec);
            AddTimes(medication.accountID, dates, medication.times);

            Medication med = new Medication
            {
                accountID = medication.accountID,
                dateRecord = medication.dateRecord,
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

            var medicationsList = InfoMedication(medication);

            return medicationsList;

        }

        public List<InfoMedicationDto> GetMedications(Guid id, DateOnly date)
        {
            throw new NotImplementedException();
        }

        public List<InfoMedicationDto> UpdateMedication(UpdateMedicationUseDto values)
        {
            var med = _bd.Medications.Find(values.medicationID);

            if (med == null)
            {
                throw new UnstoredValuesException();
            }

            bool removeSuccessful = false;

            if(med.initialFrec != values.initialFrec)
            {
                removeSuccessful = UpdateForNewDateInitial(med, values.dateRecord, values.initialFrec);
            }

            if(med.finalFrec != values.finalFrec)
            {
                removeSuccessful = UpdateForNewDateFinal(med, values.dateRecord, values.finalFrec);
            }

            if(med.dailyFrec != values.dailyFrec)
            {
                removeSuccessful = UpdateForNewDailyFrec(med, values.times, values.dateRecord, values.dailyFrec);
            }




            throw new NotImplementedException();
        }

        public List<InfoMedicationDto> DeleteMedication(Guid id)
        {
            throw new NotImplementedException();
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

        public void AddTimes(Guid id, List<DateOnly>dates, List<TimeOnly> times)
        {
            foreach (DateOnly date in dates)
            {
                foreach (TimeOnly time in times)
                {
                    Times register = new Times
                    {
                        medicationID = id,
                        dateMedication = date,
                        hours = time.Hour,
                        minutes = time.Minute,
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

        public List<InfoMedicationDto> InfoMedication(AddMedicationUseDto medication)
        {
            var recordsMedication = _bd.Medications
                                      .Where(e => e.accountID == medication.accountID && e.dateRecord == medication.dateRecord)
                                      .ToList();

            List<Guid> ids = recordsMedication.Select(e => e.medicationID).ToList();

            List<InfoMedicationDto> medicationsList = new List<InfoMedicationDto>();

            var recordsTimes = _bd.Times.Where(e => ids.Contains(e.medicationID)
                                && e.dateMedication == medication.dateRecord).ToList();


            var groupByTimes = recordsTimes.GroupBy(e => e.medicationID)
                                            .ToDictionary(g => g.Key, g => g.ToList());


            foreach (var record in recordsMedication)
            {
                var times = groupByTimes.ContainsKey(record.medicationID)
                ? _mapper.Map<List<TimeListDto>>(groupByTimes[record.medicationID])
                : new List<TimeListDto>();

                if (times.Any())
                {
                    var medicationInfo = new InfoMedicationDto
                    {
                        medicationID = medication.accountID,
                        dateRecord = medication.dateRecord,
                        nameMedication = medication.nameMedication,
                        dose = medication.dose,
                        initialFrec = medication.initialFrec,
                        finalFrec = medication.finalFrec,
                        dailyFrec = medication.dailyFrec,
                        times = times
                    };

                    medicationsList.Add(medicationInfo);
                }
            }

            return medicationsList;
        }

        public bool UpdateForNewDateInitial(Medication medication, DateOnly dateUpdate, DateOnly newInitialDate)
        {
            List<Times> records1, records2, records3;
            List<DateOnly> list1, list2, list3;

            if(!(medication.initialFrec <= newInitialDate && newInitialDate <= medication.finalFrec))
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

                return true;
            }

            if (dateUpdate <= newInitialDate)
            {
                //dateUpdate <= newInitialDate
                records3 = compiledQuery(_bd, medication.medicationID, dateUpdate, newInitialDate);

                list3 = GetDatesInRange(newInitialDate, dateUpdate);

                processRecords(records3, list3);
              
                return true;
            }
        
            return false;
        }

        public bool UpdateForNewDateFinal(Medication medication, DateOnly dateUpdate, DateOnly newFinalDate)
        {
            List<Times> records1, records2, records3, records4;
            List<DateOnly> list1, list2, list3, list4;

            if(newFinalDate <= medication.initialFrec)
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

                return true;
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

                return true;

            }

            if(medication.finalFrec <= newFinalDate)
            {
                //finalFrecPrevious <= newFinalDate
                var newRecordsToAList = GetDatesInRange(medication.finalFrec, newFinalDate);

                var timesExamples = _bd.Times.Where(e => e.medicationID == medication.medicationID).ToList();

                var groupByTimes = timesExamples.GroupBy(e => e.dateMedication)
                                   .ToDictionary(g => g.Key, g => g.Select(e => new { e.hours, e.minutes }).ToList());


                var times = groupByTimes.ContainsKey(medication.finalFrec)
                ? _mapper.Map<List<TimeOnly>>(groupByTimes[medication.finalFrec])
                : new List<TimeOnly>();

                AddTimes(medication.medicationID, newRecordsToAList, times);

                return true;

            }
            return false;
        }

        public bool UpdateForNewDailyFrec(Medication medication, List<TimeOnly> times, DateOnly dateUpdate, int newDailyFrec)
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
                AddTimes(medication.medicationID, newRecords, times);

                return true;
            }

            return false;
        }
    }
}