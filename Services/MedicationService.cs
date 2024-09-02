using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Medication;
using AppVidaSana.Models.Dtos.Medication_Dtos;
using AppVidaSana.Models.Medications;
using AppVidaSana.Services.IServices;
using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using Sprache;
using System.Collections.Generic;
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
                CreateMedication(medication.nameMedication);
            }

            Guid medicationID = _bd.Medications.FirstOrDefault(e => e.nameMedication == medication.nameMedication).medicationID;

            var periodExist = _bd.PeriodsMedications.Any(e => e.medicationID == medicationID
                                                         && e.accountID == medication.accountID
                                                         && e.initialFrec == medication.initialFrec
                                                         && e.finalFrec == medication.finalFrec
                                                         && e.dose == medication.dose
                                                         && e.timesPeriod == medication.times);

            if (periodExist) { throw new NotRepeatPeriodException(); }
            if (medication.dateActual < medication.initialFrec) { throw new UnstoredValuesException(); }
            if (medication.finalFrec < medication.initialFrec) { throw new UnstoredValuesException(); }

            PeriodsMedications period = new PeriodsMedications
            {
                medicationID = medicationID,
                accountID = medication.accountID,
                initialFrec = medication.initialFrec,
                finalFrec = medication.finalFrec,
                dose = medication.dose,
                timesPeriod = medication.times
            };

            ValidationPeriodMedication(period);

            _bd.PeriodsMedications.Add(period);

            if (!Save()) { throw new UnstoredValuesException(); }

            Guid periodID = _bd.PeriodsMedications.FirstOrDefault(e => e.accountID == medication.accountID
                                                                  && e.medicationID == medicationID
                                                                  && e.initialFrec == medication.initialFrec
                                                                  && e.finalFrec == medication.finalFrec).periodID;


            AddTimes(periodID, medication.dateActual, medication.times);
            
            var medicationsList = InfoMedicationJustAddUpdateDelete(medicationID, periodID, medication.dateActual);

            return medicationsList;

        }

        public MedicationsAndValuesGraphicDto GetMedications(Guid accountID, DateOnly dateActual)
        {
            List<InfoMedicationDto> listMedications = new List<InfoMedicationDto>();

            var periods = _bd.PeriodsMedications.Where(e => e.accountID == accountID
                                                       && e.initialFrec <= dateActual
                                                       && dateActual <= e.finalFrec).ToList();

            var groupPeriodsByMedicationID = periods.GroupBy(obj => obj.medicationID)
                                                    .ToDictionary(
                                                        g => g.Key,
                                                        g => g.Where(e => e.medicationID == g.Key)
                                                    );

            foreach (var med in groupPeriodsByMedicationID)
            {
                var medication = _bd.Medications.Find(med.Key);

                var periodsForMedication = med.Value;

                List<Times> timesForMedication = new List<Times>();

                foreach (var period in periodsForMedication)
                {
                    var times = _bd.Times.Where(e => e.periodID == period.periodID
                                                && e.dateMedication == dateActual).ToList();

                    if (!times.Any())
                    {
                        AddTimes(period.periodID, dateActual, period.timesPeriod);

                        times = _bd.Times.Where(e => e.periodID == period.periodID
                                                && e.dateMedication == dateActual).ToList();
                    }

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

                }
            }

            int countStatus = 0, totalMedications = 0, medicationsConsumed = 0;
            bool statusGeneral = false;
            List<WeeklyAttachmentDto> weeklyList = new List<WeeklyAttachmentDto>();
            DateOnly dateFinal = dateActual.AddDays(-6);

            var getTimes = from pMed in _bd.Set<PeriodsMedications>()
                           join t in _bd.Set<Times>()
                           on pMed.periodID equals t.periodID
                           where pMed.accountID == accountID
                           select new { t };

            var timeList = getTimes.ToList();

            timeList.OrderBy(x => x.t.time).ToList();

            var groupObjectsByID = timeList.GroupBy(obj => obj.t.periodID)
                                            .ToDictionary(
                                                g => g.Key,
                                                g => g.ToList()
                                            );

            List<DateOnly> dates = GetDatesInRange(dateFinal, dateActual);

            foreach (var date in dates)
            {
                foreach (var time in groupObjectsByID)
                {
                    countStatus = 0;

                    var list = time.Value.Where(e => e.t.dateMedication == date).ToList();

                    if (list.Any())
                    {
                        totalMedications++;

                        foreach (var l in list)
                        {
                            if (l.t.medicationStatus) { countStatus++; }
                        }

                        statusGeneral = (countStatus == list.Count()) ? true : false;

                        if (statusGeneral) { medicationsConsumed++; }
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

            var period = _bd.PeriodsMedications.Find(values.periodID);

            var medication = _bd.Medications.Find(values.medicationID);

            if (medication == null) { throw new UnstoredValuesException(); }

            if (medication.nameMedication != values.nameMedication)
            {
                var existNameMedication = _bd.Medications.FirstOrDefault(e => e.nameMedication == values.nameMedication);

                if(existNameMedication != null)
                {
                    period.medicationID = existNameMedication.medicationID;

                    ValidationPeriodMedication(period);

                    _bd.PeriodsMedications.Update(period);

                    if (!Save()) { throw new UnstoredValuesException(); }
                }
                else
                {
                    CreateMedication(values.nameMedication);
                    Guid medicationID = _bd.Medications
                                        .FirstOrDefault(e => e.nameMedication == values.nameMedication).medicationID;
                    period.medicationID = medicationID;

                    ValidationPeriodMedication(period);

                    _bd.PeriodsMedications.Update(period);

                    if (!Save()) { throw new UnstoredValuesException(); }
                }
            }

            if (values.updateDate < values.initialFrec) { throw new UnstoredValuesException(); }

            infoMedication = UpdateForNewDailyFrec(values, period.periodID);
            
            if (period.initialFrec != values.initialFrec || period.finalFrec != values.finalFrec)
            {
                infoMedication =  UpdateForNewDateInitialAndFinal(period, 
                                                                  values.updateDate, values.initialFrec, values.finalFrec);
            }
  
            period.dose = values.dose;

            ValidationPeriodMedication(period);

            _bd.PeriodsMedications.Update(period);

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
            var recordsToDelete = _bd.Times.Where(e => e.periodID == id && e.dateMedication >= date).ToList();
               
            _bd.Times.RemoveRange(recordsToDelete);

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

            if (endDate < startDate) { throw new ErrorRangeDatesException(); }

            return dates;
        }

        private void CreateMedication(string nameMedication)
        {
            Medication newMedication = new Medication
            {
                nameMedication = nameMedication
            };

            ValidationMedication(newMedication);

            _bd.Medications.Add(newMedication);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private void AddTimes(Guid periodID, DateOnly dateActual, string times)
        {
            List<Times> newTimes = new List<Times>();

            string[] subs = times.Split(',');

            foreach (var sub in subs)
            {
                Times time = new Times
                {
                    periodID = periodID,
                    dateMedication = dateActual,
                    time = TimeOnly.Parse(sub),
                    medicationStatus = false
                };

                ValidationTime(time);

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
                                                  && e.dateMedication == date
                                                  && e.time == time);

                    if (timeExist) {
                        var timeToDelete = _bd.Times.Where(e => e.periodID == periodID
                                                          && e.dateMedication == date
                                                          && e.time == time).ToList();

                        _bd.Times.RemoveRange(timeToDelete);
                        if (!Save()) { throw new UnstoredValuesException(); }
                    }

                    Times newTime = new Times
                    {
                        periodID = periodID,
                        dateMedication = date,
                        time = time,
                        medicationStatus = false
                    };

                    ValidationTime(newTime);

                    newTimes.Add(newTime);
                }
            }

            _bd.Times.AddRange(newTimes);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private InfoMedicationDto UpdateForNewDateInitialAndFinal(PeriodsMedications periods, 
                                                                  DateOnly dateRecord, DateOnly newInitialDate, 
                                                                  DateOnly newFinalDate)
        {

            if(newFinalDate < newInitialDate) { throw new UnstoredValuesException(); }

            if (newInitialDate < periods.initialFrec)
            {
                List<TimeOnly> times = new List<TimeOnly>();

                var dates = GetDatesInRange(newInitialDate, periods.initialFrec.AddDays(-1));

                var timesExample = _bd.Times.Where(e => e.periodID == periods.periodID
                                                   && e.dateMedication == periods.initialFrec).ToList();

                times.AddRange(timesExample.Select(e => e.time));

                CreateTimes(dates, times, periods.periodID);

                periods.initialFrec = newInitialDate;

                _bd.PeriodsMedications.Update(periods);

                if (!Save()) { throw new UnstoredValuesException(); }

            }

            if (periods.finalFrec < newFinalDate)
            {
                List<TimeOnly> times = new List<TimeOnly>();

                var dates = GetDatesInRange(periods.finalFrec.AddDays(1), newFinalDate);

                var timesExample = _bd.Times.Where(e => e.periodID == periods.periodID
                                                   && e.dateMedication == periods.initialFrec).ToList();

                times.AddRange(timesExample.Select(e => e.time));

                CreateTimes(dates, times, periods.periodID);

                periods.finalFrec = newFinalDate;

                _bd.PeriodsMedications.Update(periods);

                if (!Save()) { throw new UnstoredValuesException(); }
            }

            periods.initialFrec = newInitialDate;
            periods.finalFrec = newFinalDate;

            _bd.PeriodsMedications.Update(periods);

            if (!Save()) { throw new UnstoredValuesException(); }

            return InfoMedicationJustAddUpdateDelete(periods.medicationID, periods.periodID, dateRecord);
        }

        private InfoMedicationDto UpdateForNewDailyFrec(UpdateMedicationUseDto values, Guid periodID)
        {
            List<Guid> idsPrevious = new List<Guid>();
            List<Guid> ids = new List<Guid>();
            List<DateOnly> dates;

            Action<List<TimeListDto>, DateOnly> processRecords = (list, date) =>
            {
                foreach (var id in list)
                {
                    var record = _bd.Times.Find(id.timeID);

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

                var recordsTimes = _bd.Times.Where(e => e.dateMedication == values.updateDate
                                                  && e.periodID == values.periodID).ToList();

                idsPrevious.AddRange(recordsTimes.Select(e => e.timeID));

                foreach (var item in values.times)
                {
                    ids.Add(item.timeID);
                }

                var idsToKeep = idsPrevious.Intersect(ids).ToList();
                var idsToKeepString = string.Join(",", idsToKeep.Select(id => _bd.Times.Find(id).time.ToString("HH:mm")));

                var findIdsToDelete = idsPrevious.Except(ids).ToList();

                foreach (var id in findIdsToDelete)
                {
                    var recordTime = _bd.Times.Find(id);

                    var recordsToDelete = _bd.Times.Where(e => e.periodID == periodID
                                                          && e.time == recordTime.time
                                                          && e.dateMedication >= values.updateDate).ToList();

                    _bd.Times.RemoveRange(recordsToDelete);

                    if (!Save()) { throw new UnstoredValuesException(); }
                }

                var period = _bd.PeriodsMedications.Find(periodID);

                period.timesPeriod = idsToKeepString;

                _bd.PeriodsMedications.Update(period);

                if (!Save()) { throw new UnstoredValuesException(); }


                return InfoMedicationJustAddUpdateDelete(period.medicationID, periodID, values.updateDate);
            }
            else
            {
                processRecords(values.times, values.updateDate);

                List<Times> newTimes = new List<Times>();

                string[] subs = values.newTimes.Split(',');

                foreach (var sub in subs)
                { 
                    var timeExist = _bd.Times.Any(e => e.periodID == periodID
                                                  && e.dateMedication == values.updateDate
                                                  && e.time == TimeOnly.Parse(sub));

                    if (!timeExist)
                    {
                        Times time = new Times
                        {
                            periodID = periodID,
                            dateMedication = values.updateDate,
                            time = TimeOnly.Parse(sub),
                            medicationStatus = false
                        };

                        ValidationTime(time);

                        newTimes.Add(time);
                    }
                }

                _bd.Times.AddRange(newTimes);

                if (!Save()) { throw new UnstoredValuesException(); }

                var period = _bd.PeriodsMedications.Find(periodID);

                period.timesPeriod = period.timesPeriod + " ," + values.newTimes;

                _bd.PeriodsMedications.Update(period);

                if (!Save()) { throw new UnstoredValuesException(); }

                return InfoMedicationJustAddUpdateDelete(period.medicationID, periodID, values.updateDate);
            }
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
    }
}