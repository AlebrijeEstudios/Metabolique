using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Models.Seguimientos_Mensuales.Resultados;
using AppVidaSana.Services.IServices.IMonthly_Follow_Ups;
using Azure;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Services.Monthly_Follows_Ups
{
    public class MFUsHabitsService : IMFUsHabits
    {
        private readonly AppDbContext _bd;

        public MFUsHabitsService(AppDbContext bd)
        {
            _bd = bd;
        }

        public RetrieveResponsesHabitsDto RetrieveAnswers(Guid id, string month, int year)
        {
            var responses = _bd.MFUsHabits.FirstOrDefault(c => c.accountID == id && c.month == month && c.year == year);

            if (responses == null)
            {
                throw new UserNotFoundException();
            }

            var results = _bd.resultsHabits.FirstOrDefault(c => c.monthlyFollowUpID == responses.monthlyFollowUpID);

            if (results == null)
            {
                throw new ResultsNotFoundException();
            }

            RetrieveResponsesHabitsDto res = new RetrieveResponsesHabitsDto
            {
                monthlyFollowUpID = responses.monthlyFollowUpID,
                month = responses.month,
                year = responses.year,
                answerQuestion1 = responses.answerQuestion1,
                answerQuestion2 = responses.answerQuestion2,
                answerQuestion3 = responses.answerQuestion3,
                answerQuestion4 = responses.answerQuestion4,
                answerQuestion5a = responses.answerQuestion5a,
                answerQuestion5b = responses.answerQuestion5b,
                answerQuestion5c = responses.answerQuestion5c,
                answerQuestion5d = responses.answerQuestion5d,
                answerQuestion5e = responses.answerQuestion5e,
                answerQuestion5f = responses.answerQuestion5f,
                answerQuestion5g = responses.answerQuestion5g,
                answerQuestion5h = responses.answerQuestion5h,
                answerQuestion5i = responses.answerQuestion5i,
                answerQuestion5j = responses.answerQuestion5j,
                answerQuestion6 = responses.answerQuestion6,
                answerQuestion7 = responses.answerQuestion7,
                answerQuestion8 = responses.answerQuestion8,
                answerQuestion9 = responses.answerQuestion9,
                resultComponent1 = results.resultComponent1,
                resultComponent2 = results.resultComponent2,
                resultComponent3 = results.resultComponent3,
                resultComponent4 = results.resultComponent4,
                resultComponent5 = results.resultComponent5,
                resultComponent6 = results.resultComponent6,
                resultComponent7 = results.resultComponent7,
                globalClassification = results.globalClassification,
                classification = results.classification
            };

            return res;
        }

        public SaveResultsDto SaveAnswers(SaveResponsesHabitsDto res)
        {
            var account = _bd.Accounts.Find(res.accountID);

            if (account == null)
            {
                throw new UserNotFoundException();
            }

            int resultComponent1 = res.answerQuestion6;
            int resultComponent2 = component2(res.answerQuestion2, res.answerQuestion5a);
            int resultComponent3 = res.answerQuestion4;
            int resultComponent4 = component4(res.answerQuestion1, res.answerQuestion3, res.answerQuestion4);
            int resultComponent5 = component5(res);
            int resultComponent6 = res.answerQuestion7;
            int resultComponent7 = component7(res.answerQuestion8, res.answerQuestion9);

            int total = resultComponent1 + resultComponent2 + resultComponent3 + resultComponent4 +
                        resultComponent5 + resultComponent6 + resultComponent7;

            string classificationPSQI = classification(total);

            MFUsHabits answers = new MFUsHabits
            {
                accountID = res.accountID,
                month = res.month,
                year = res.year,
                answerQuestion1 = res.answerQuestion1,
                answerQuestion2 = res.answerQuestion2,
                answerQuestion3 = res.answerQuestion3,
                answerQuestion4 = res.answerQuestion4,
                answerQuestion5a = res.answerQuestion5a,
                answerQuestion5b = res.answerQuestion5b,
                answerQuestion5c = res.answerQuestion5c,
                answerQuestion5d = res.answerQuestion5d,
                answerQuestion5e = res.answerQuestion5e,
                answerQuestion5f = res.answerQuestion5f,
                answerQuestion5g = res.answerQuestion5g,
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


        public static int component2(int response2, int response5a)
        {
            int totalTest, value = 0;
            
            totalTest = response2 + response5a;

            if (totalTest == 0) { return value; }
            if (totalTest == 1 || totalTest == 2) { value = 1; }
            if (totalTest == 3 || totalTest == 4) { value = 2; }
            if (totalTest == 5 || totalTest == 6) { value = 3; }

            return value;
        }

        public static int component4(TimeOnly response1, TimeOnly response3, int response4)
        {
            int value = 0;
            TimeSpan start = response1.ToTimeSpan();
            TimeSpan end = response3.ToTimeSpan();

            if (end < start)
            {
                start += TimeSpan.FromDays(1);
            }

            TimeSpan diff = start - end;

            int bedHours = (int)diff.TotalHours;

            float ES = ((float)response4 / bedHours) * 100;

            if (ES > 85) { return value; }

            if (ES >= 75 && ES <= 84) { value = 1; }

            if (ES >= 65 && ES <= 74) { value = 2; }

            if (ES < 65) { value = 3; }

            return value;
        }

        public static int component5(SaveResponsesHabitsDto response)
        { 
            int value = 0, totalTest;

            totalTest = response.answerQuestion5b +
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

        public static int component7(int response8, int response9)
        {
            int value = 0, totalTest;

            totalTest = response8 + response9;

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
