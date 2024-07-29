using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Models.Seguimientos_Mensuales.Resultados;
using AppVidaSana.Services.IServices.IMonthly_Follow_Ups;
using AutoMapper;
using Azure;
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

        public RetrieveResponsesHabitsDto RetrieveAnswers(Guid id, string month, int year)
        {
            var responses = _bd.MFUsHabits.FirstOrDefault(c => c.accountID == id && c.month == month && c.year == year);

            RetrieveResponsesHabitsDto res;

            if(responses == null)
            {
                res = null;
                return res;
            }

            var results = _bd.resultsHabits.FirstOrDefault(c => c.monthlyFollowUpID == responses.monthlyFollowUpID);

            if (results == null)
            {
                res = null;
                return res;
            }

            res = _mapper.Map<RetrieveResponsesHabitsDto>(responses);
            _mapper.Map(results, res);

            return res;
        }

        public SaveResultsDto SaveAnswers(SaveResponsesHabitsDto res)
        {
            var answersExisting = _bd.MFUsHabits.Count(e => e.accountID == res.accountID &&
                                    e.month == res.month && e.year == res.year);

            if (answersExisting > 0)
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

            var answers = _mapper.Map<MFUsHabits>(res);

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

            SaveResultsDto results = new SaveResultsDto
            {
                accountID = res.accountID,
                month = res.month,
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

        public string SaveResults(SaveResultsDto res)
        {
            var mfusHabit = _bd.MFUsHabits.FirstOrDefault(c => c.accountID == res.accountID
                            && c.month == res.month && c.year == res.year);

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

            _bd.resultsHabits.Add(results);

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


        public static byte component2(byte response2, byte response5a)
        {
            byte value = 0;
            int totalTest = response2 + response5a;

            if (totalTest == 0) { return value; }
            if (totalTest == 1 || totalTest == 2) { value = 1; }
            if (totalTest == 3 || totalTest == 4) { value = 2; }
            if (totalTest == 5 || totalTest == 6) { value = 3; }

            return value;
        }

        public static byte component3(int response4)
        {
            byte value = 0;

            if ((float) response4 > 7) { return value; }

            if ((float) response4 >= 6 && (float) response4 <= 7) { value = 1; }

            if ((float) response4 >= 5 && (float) response4 <= 6) { value = 2; }

            if ((float) response4 < 5) { value = 3; }

            return value;
        }

        public static byte component4(TimeOnly response1, TimeOnly response3, int response4)
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

        public static byte component5(SaveResponsesHabitsDto response)
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

        public static byte component7(byte response8, byte response9)
        {
            byte value = 0;
            int totalTest = response8 + response9;

            if (totalTest == 0) { return value; }

            if (totalTest == 1 || totalTest == 2) { value = 1; }

            if (totalTest == 3 || totalTest == 4) { value = 2; }

            if (totalTest == 5 || totalTest == 6) { value = 3; }

            return value;
        }

        public static string classification(int totalGlobal)
        {
            string value = (totalGlobal <= 5) 
                ? "Buena calidad del sueño" : "Mala calidad del sueño";

            return value;
        }

    }
}
