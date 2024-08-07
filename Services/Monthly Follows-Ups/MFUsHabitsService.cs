﻿using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;
using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Models.Seguimientos_Mensuales.Resultados;
using AppVidaSana.Services.IServices.IMonthly_Follow_Ups;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Services.Monthly_Follows_Ups
{
    public class MFUsHabitsService : IMFUsHabits
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public MFUsHabitsService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
        }

        public RetrieveResponsesHabitsDto RetrieveAnswers(Guid id, int month, int year)
        {
            var responseMapping = new Dictionary<int, string>
            {
                { 1, "Enero" },
                { 2, "Febrero" },
                { 3, "Marzo" },
                { 4, "Abril" },
                { 5, "Mayo" },
                { 6, "Junio" },
                { 7, "Julio" },
                { 8, "Agosto" },
                { 9, "Septiembre" },
                { 10, "Octubre" },
                { 11, "Noviembre" },
                { 12, "Diciembre" }
            };

            var existDate = responseMapping.ContainsKey(month)
                ? responseMapping[month] : "Mes no existente";


            if (existDate == "Mes no existente")
            {
                throw new UnstoredValuesException();

            }

            RetrieveResponsesHabitsDto response;

            var monthRecord = _bd.Months.FirstOrDefault(e => e.month == responseMapping[month] && e.year == year);

            if (monthRecord == null)
            {
                response = new RetrieveResponsesHabitsDto();
                return response;
            }

            var records = _bd.MFUsHabits.FirstOrDefault(c => c.accountID == id && c.monthID == monthRecord.monthID);

            if (records == null)
            {
                response = new RetrieveResponsesHabitsDto();
                return response;
            }

            var results = _bd.ResultsHabits.FirstOrDefault(c => c.monthlyFollowUpID == records.monthlyFollowUpID);

            if (results == null)
            {
                response = new RetrieveResponsesHabitsDto();
                return response;
            }

            response = _mapper.Map<RetrieveResponsesHabitsDto>(records);
            response = _mapper.Map(monthRecord, response);
            _mapper.Map(results, response);

            return response;
        }

        public SaveResultsHabitsDto SaveAnswers(SaveResponsesHabitsDto res)
        {
            var responseMapping = new Dictionary<int, string>
            {
                { 1, "Enero" },
                { 2, "Febrero" },
                { 3, "Marzo" },
                { 4, "Abril" },
                { 5, "Mayo" },
                { 6, "Junio" },
                { 7, "Julio" },
                { 8, "Agosto" },
                { 9, "Septiembre" },
                { 10, "Octubre" },
                { 11, "Noviembre" },
                { 12, "Diciembre" }
            };

            var existDate = responseMapping.ContainsKey(res.month)
               ? responseMapping[res.month] : "Mes no existente";

            if (existDate == "Mes no existente")
            {
                throw new UnstoredValuesException();

            }

            var existRecord = _bd.Months.Any(e => e.month == responseMapping[res.month] && e.year == res.year);

            if (!existRecord)
            {
                MFUsMonths month = new MFUsMonths
                {
                    month = responseMapping[res.month],
                    year = res.year
                };

                _bd.Months.Add(month);

                if (!Save())
                {
                    throw new UnstoredValuesException();
                }
            }

            Guid monthID = _bd.Months.FirstOrDefault(e => e.month == responseMapping[res.month] && e.year == res.year).monthID;

            var answersExisting = _bd.MFUsHabits.Any(e => e.accountID == res.accountID &&
                                    e.monthID == monthID);

            if (answersExisting)
            {
                throw new RepeatRegistrationException();
            }

            var account = _bd.Accounts.Find(res.accountID);

            if (account == null)
            {
                throw new UserNotFoundException();
            }

            byte resultComponent1 = res.answerQuestion6;
            byte resultComponent2 = component2(res.answerQuestion2, res.answerQuestion5a);
            byte resultComponent3 = component3(res.answerQuestion4);
            byte resultComponent4 = component4(res.answerQuestion1, res.answerQuestion3, res.answerQuestion4);
            byte resultComponent5 = component5(res);
            byte resultComponent6 = res.answerQuestion7;
            byte resultComponent7 = component7(res.answerQuestion8, res.answerQuestion9);

            int total = resultComponent1 + resultComponent2 + resultComponent3 + resultComponent4 +
                        resultComponent5 + resultComponent6 + resultComponent7;

            string classificationPSQI = classification(total);

            MFUsHabits answers = new MFUsHabits
            {
                accountID = res.accountID,
                monthID = monthID,
                answerQuestion1 = res.answerQuestion1,
                answerQuestion2 = res.answerQuestion2,
                answerQuestion3 = res.answerQuestion3,
                answerQuestion4 = res.answerQuestion4,
                answerQuestion5a = res.answerQuestion5a,
                answerQuestion5b= res.answerQuestion5b,
                answerQuestion5c = res.answerQuestion5c,
                answerQuestion5d = res.answerQuestion5d,
                answerQuestion5e = res.answerQuestion5e,
                answerQuestion5f = res.answerQuestion5f,
                answerQuestion5h = res.answerQuestion5h,
                answerQuestion5i = res.answerQuestion5i,
                answerQuestion5j = res.answerQuestion5j,
                answerQuestion6 = res.answerQuestion6,
                answerQuestion7 = res.answerQuestion7,
                answerQuestion8 = res.answerQuestion8,
                answerQuestion9 = res.answerQuestion9
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(answers, null, null);

            if (!Validator.TryValidateObject(answers, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }

            _bd.MFUsHabits.Add(answers);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            SaveResultsHabitsDto results = new SaveResultsHabitsDto
            {
                accountID = res.accountID,
                month = responseMapping[res.month],
                year = res.year,
                resultComponent1 = resultComponent1,
                resultComponent2 = resultComponent2,
                resultComponent3 = resultComponent3,
                resultComponent4 = resultComponent4,
                resultComponent5 = resultComponent5,
                resultComponent6 = resultComponent6,
                resultComponent7 = resultComponent7,
                globalClassification = total,
                classification = classificationPSQI
            };

            return results;

        }

        public string SaveResults(SaveResultsHabitsDto res)
        {

            Guid monthID = _bd.Months.FirstOrDefault(e => e.month == res.month && e.year == res.year).monthID;

            var mfusHabit = _bd.MFUsHabits.FirstOrDefault(c => c.accountID == res.accountID
                            && c.monthID == monthID);

            if (mfusHabit == null)
            {
                throw new HabitNotFoundException();
            }

            HabitsResults results = new HabitsResults
            {
                monthlyFollowUpID = mfusHabit.monthlyFollowUpID,
                resultComponent1 = res.resultComponent1,
                resultComponent2 = res.resultComponent2,
                resultComponent3 = res.resultComponent3,
                resultComponent4 = res.resultComponent4,
                resultComponent5 = res.resultComponent5,
                resultComponent6 = res.resultComponent6,
                resultComponent7 = res.resultComponent7,
                globalClassification = res.globalClassification,
                classification = res.classification
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(results, null, null);

            if (!Validator.TryValidateObject(results, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }

            _bd.ResultsHabits.Add(results);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return "Sus respuestas han sido guardadas correctamente";
        }

        public bool Save()
        {
            try {
                return _bd.SaveChanges() >= 0;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static byte component2(byte response2, byte response5a)
        {
            byte value = 0;
            int totalTest = response2 + response5a;

            if (totalTest == 0) { return value; }
            if (totalTest == 1 || totalTest == 2) { value = 1; }
            if (totalTest == 3 || totalTest == 4) { value = 2; }
            if (totalTest == 5 || totalTest == 6) { value = 3; }

            return value;
        }

        private static byte component3(int response4)
        {
            byte value = 0;

            if ((float) response4 > 7) { return value; }

            if ((float) response4 >= 6 && (float) response4 <= 7) { value = 1; }

            if ((float) response4 >= 5 && (float) response4 <= 6) { value = 2; }

            if ((float) response4 < 5) { value = 3; }

            return value;
        }

        private static byte component4(TimeOnly response1, TimeOnly response3, int response4)
        {
            byte value = 0;
            TimeSpan start = response1.ToTimeSpan();
            TimeSpan end = response3.ToTimeSpan();

            if (end < start)
            {
                end += TimeSpan.FromDays(1);
            }

            TimeSpan diff = end - start;

            int bedHours = (int) diff.TotalHours;

            if (bedHours == 0)
            {
                return 3;
            }

            float ES = ((float) response4 / bedHours) * 100;

            if (ES > 85) { return value; }

            if (ES >= 75 && ES <= 84) { value = 1; }

            if (ES >= 65 && ES <= 74) { value = 2; }

            if (ES < 65) { value = 3; }

            return value;
        }

        private static byte component5(SaveResponsesHabitsDto response)
        {
            byte value = 0;
            int totalTest = response.answerQuestion5b +
                        response.answerQuestion5c +
                        response.answerQuestion5d +
                        response.answerQuestion5e +
                        response.answerQuestion5f +
                        response.answerQuestion5g +
                        response.answerQuestion5h +
                        response.answerQuestion5i +
                        response.answerQuestion5j;

            if (totalTest == 0) { return value; }

            if (totalTest >= 1 && totalTest <= 9) { value = 1; }

            if (totalTest >= 10 && totalTest <= 18) { value = 2; }

            if (totalTest >= 19 && totalTest <= 27) { value = 3; }

            return value;
        }

        private static byte component7(byte response8, byte response9)
        {
            byte value = 0;
            int totalTest = response8 + response9;

            if (totalTest == 0) { return value; }

            if (totalTest == 1 || totalTest == 2) { value = 1; }

            if (totalTest == 3 || totalTest == 4) { value = 2; }

            if (totalTest == 5 || totalTest == 6) { value = 3; }

            return value;
        }

        private static string classification(int totalGlobal)
        {
            string value = (totalGlobal <= 5) 
                ? "Buena calidad del sueño" : "Mala calidad del sueño";

            return value;
        }

    }
}
