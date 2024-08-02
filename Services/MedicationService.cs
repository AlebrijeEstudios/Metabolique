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

        public List<InfoMedicationDto> UpdateMedication(InfoMedicationDto values)
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

            if(!(medication.initialFrec <= newInitialDate && newInitialDate <= medication.finalFrec))
            {
                throw new UnstoredValuesException();
            }

            Func<AppDbContext, Guid , DateOnly, DateOnly, List<Times>> compiledQuery = EF.CompileQuery(
                        (AppDbContext context, Guid id, DateOnly date1, DateOnly date2) =>
                           context.Times.Where(e => date1 <= e.dateMedication && e.dateMedication <= date2
                                                && e.medicationID == id).ToList()
                        );


            if (medication.initialFrec <= newInitialDate && newInitialDate <= dateUpdate) {
                

                records1 = _bd.Times.Where(e => e.dateMedication <= newInitialDate 
                                    && e.medicationID == medication.medicationID).ToList();

                List<DateOnly> list1 = GetDatesInRange(medication.initialFrec, newInitialDate);

                var groupByDates = records1.GroupBy(e => e.dateMedication)
                                            .ToDictionary(g => g.Key, g => g.Select(e => e.medicationStatus).ToList());


                foreach(var date in list1)
                {
                    int countStatusConsumed = 0;

                    var times = groupByDates.ContainsKey(date);

                    if (times)
                    {
                        foreach(var status in groupByDates[date])
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

                records2 = compiledQuery(_bd, medication.medicationID, newInitialDate, dateUpdate);

                List<DateOnly> list2 = GetDatesInRange(newInitialDate, dateUpdate);

                var groupByDates2 = records2.GroupBy(e => e.dateMedication)
                                            .ToDictionary(g => g.Key, g => g.Select(e => e.medicationStatus).ToList());


                foreach (var date in list2)
                {
                    int countStatusConsumed = 0;

                    var times = groupByDates2.ContainsKey(date);

                    if (times)
                    {
                        foreach (var status in groupByDates2[date])
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

                return true;
            }

            if (dateUpdate <= newInitialDate)
            {
                records3 = compiledQuery(_bd, medication.medicationID, dateUpdate, newInitialDate);

                List<DateOnly> list3 = GetDatesInRange(newInitialDate, dateUpdate);

                var groupByDates = records3.GroupBy(e => e.dateMedication)
                                            .ToDictionary(g => g.Key, g => g.Select(e => e.medicationStatus).ToList());


                foreach (var date in list3)
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

                return true;
            }
        

            // f1 < fNEW
            //fNEW < fUpd
            //fUpd < fNEW

            return false;
        }

        public bool UpdateForNewDateFinal(Medication medication, DateOnly dateUpdate, DateOnly newFinalDate)
        {
            //fiAnterior < fiNueva Agregara mas dnetro de este rango
            //fiNueva < fiAnterior Eliminar los regsitrso que se encunetren aqui
            //dateUpdate < fiNueva (del caso anterior) Igual borar rgeitros 
            //fiNueva < dateUpadte Igual borras regitros 
            //fI < fiNueva Igual borar regitros

            if(newFinalDate <= medication.initialFrec)
            {
                throw new UnstoredValuesException();
            }

            Func<AppDbContext, Guid , DateOnly, DateOnly, List<Times>> compiledQuery = EF.CompileQuery(
                        (AppDbContext context, Guid id, DateOnly date1, DateOnly date2) =>
                           context.Times.Where(e => date1 <= e.dateMedication && e.dateMedication <= date2
                                                && e.medicationID == id).ToList()
                        );


            List<Times> records1, records2, records3, records4;

            if (dateUpdate <= newFinalDate && newFinalDate <= medication.finalFrec)
            {
                records1 = compiledQuery(_bd, medication.medicationID, dateUpdate, newFinalDate);

                List<DateOnly> list1 = GetDatesInRange(dateUpdate, newFinalDate);

                var groupByDates = records1.GroupBy(e => e.dateMedication)
                                            .ToDictionary(g => g.Key, g => g.Select(e => e.medicationStatus).ToList());


                foreach (var date in list1)
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

                records2 = compiledQuery(_bd, medication.medicationID, newFinalDate, medication.finalFrec);

                List<DateOnly> list2 = GetDatesInRange(newFinalDate, dateUpdate);

                var groupByDates2 = records2.GroupBy(e => e.dateMedication)
                                            .ToDictionary(g => g.Key, g => g.Select(e => e.medicationStatus).ToList());


                foreach (var date in list2)
                {
                    int countStatusConsumed = 0;

                    var times = groupByDates2.ContainsKey(date);

                    if (times)
                    {
                        foreach (var status in groupByDates2[date])
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

                return true;
            }

            if(medication.initialFrec <= newFinalDate && newFinalDate <= dateUpdate)
            {
                records3 = compiledQuery(_bd, medication.medicationID, medication.initialFrec, newFinalDate);

                List<DateOnly> list3 = GetDatesInRange(dateUpdate, newFinalDate);

                var groupByDates = records3.GroupBy(e => e.dateMedication)
                                            .ToDictionary(g => g.Key, g => g.Select(e => e.medicationStatus).ToList());


                foreach (var date in list3)
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

                records4 = compiledQuery(_bd, medication.medicationID, newFinalDate, dateUpdate);

                List<DateOnly> list4 = GetDatesInRange(newFinalDate, dateUpdate);

                var groupByDates2 = records4.GroupBy(e => e.dateMedication)
                                            .ToDictionary(g => g.Key, g => g.Select(e => e.medicationStatus).ToList());


                foreach (var date in list4)
                {
                    int countStatusConsumed = 0;

                    var times = groupByDates2.ContainsKey(date);

                    if (times)
                    {
                        foreach (var status in groupByDates2[date])
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

                return true;

            }

            if(medication.finalFrec <= newFinalDate)
            {
                var newRecordsToAList = GetDatesInRange(medication.finalFrec, newFinalDate);

                var timesExamples = _bd.Times.Where(e => e.medicationID == medication.medicationID).ToList();

                var groupByTimes = timesExamples.GroupBy(e => e.dateMedication)
                                            .ToDictionary(g => g.Key, g => g.Select(e => new { e.hours, e.minutes}).ToList());


                var times = groupByTimes.ContainsKey(medication.finalFrec)
                ? _mapper.Map<List<TimeOnly>>(groupByTimes[medication.finalFrec])
                : new List<TimeOnly>();


                AddTimes(medication.medicationID, newRecordsToAList, times);

                return true;

            }
            return false;
        }

    }
}