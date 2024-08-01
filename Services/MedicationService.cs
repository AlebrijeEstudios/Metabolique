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

            throw new NotImplementedException();
        }

        public List<InfoMedicationDto> DeleteMedication(Guid id)
        {
            throw new NotImplementedException();
        }

        public bool Save()
        {
            throw new NotImplementedException();
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


            var timesGroupByMedication = recordsTimes.GroupBy(e => e.medicationID)
                                            .ToDictionary(g => g.Key, g => g.ToList());


            foreach (var record in recordsMedication)
            {
                var times = timesGroupByMedication.ContainsKey(record.medicationID)
                ? _mapper.Map<List<TimeListDto>>(timesGroupByMedication[record.medicationID])
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
    }
}