using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;
using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Models.Monthly_Follow_Ups.Results;
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
            var months = new Dictionary<int, string>
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

            var getMonth = months.ContainsKey(month) ? months[month] : "Mes no existente";

            if (getMonth == "Mes no existente") { throw new UnstoredValuesException(); }

            RetrieveResponsesHabitsDto responses;

            var existMonth = _bd.Months.FirstOrDefault(e => e.month == months[month] && e.year == year);

            if (existMonth == null)
            {
                responses = null;
                return responses;
            }

            var mfuHabits = _bd.MFUsHabits.FirstOrDefault(c => c.accountID == id && c.monthID == existMonth.monthID);

            if (mfuHabits == null)
            {
                responses = null;
                return responses;
            }

            var mfuHabitsResults = _bd.ResultsHabits.FirstOrDefault(c => c.monthlyFollowUpID == mfuHabits.monthlyFollowUpID);

            if (mfuHabitsResults == null)
            {
                responses = null;
                return responses;
            }

            responses = _mapper.Map<RetrieveResponsesHabitsDto>(mfuHabits);
            responses = _mapper.Map(existMonth, responses);
            _mapper.Map(mfuHabitsResults, responses);

            return responses;
        }

        public RetrieveResponsesHabitsDto SaveAnswers(SaveResponsesHabitsDto values)
        {
            var months = new Dictionary<int, string>
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

            var getMonth = months.ContainsKey(values.month) ? months[values.month] : "Mes no existente";

            if (getMonth == "Mes no existente") { throw new UnstoredValuesException(); }

            var existMonth = _bd.Months.Any(e => e.month == months[values.month] && e.year == values.year);

            if (!existMonth)
            {
                MFUsMonths month = new MFUsMonths
                {
                    month = months[values.month],
                    year = values.year
                };

                _bd.Months.Add(month);

                if (!Save()) { throw new UnstoredValuesException(); }
            }

            Guid monthID = _bd.Months.FirstOrDefault(e => e.month == months[values.month] && e.year == values.year).monthID;

            var answersExisting = _bd.MFUsHabits.Any(e => e.accountID == values.accountID && e.monthID == monthID);

            if (answersExisting) { throw new RepeatRegistrationException(); }

            var accountExisting = _bd.Accounts.Find(values.accountID);

            if (accountExisting == null) { throw new UserNotFoundException(); }

            MFUsHabits answers = new MFUsHabits
            {
                accountID = values.accountID,
                monthID = monthID,
                answerQuestion1 = values.answerQuestion1,
                answerQuestion2 = values.answerQuestion2,
                answerQuestion3 = values.answerQuestion3,
                answerQuestion4 = values.answerQuestion4,
                answerQuestion5a = values.answerQuestion5a,
                answerQuestion5b= values.answerQuestion5b,
                answerQuestion5c = values.answerQuestion5c,
                answerQuestion5d = values.answerQuestion5d,
                answerQuestion5e = values.answerQuestion5e,
                answerQuestion5f = values.answerQuestion5f,
                answerQuestion5h = values.answerQuestion5h,
                answerQuestion5i = values.answerQuestion5i,
                answerQuestion5j = values.answerQuestion5j,
                answerQuestion6 = values.answerQuestion6,
                answerQuestion7 = values.answerQuestion7,
                answerQuestion8 = values.answerQuestion8,
                answerQuestion9 = values.answerQuestion9
            };

            ValidationSaveAnswers(answers);

            _bd.MFUsHabits.Add(answers);

            if (!Save()) { throw new UnstoredValuesException(); }

            var answersRecentlyAdd = _bd.MFUsHabits.FirstOrDefault(e => e.accountID == values.accountID && e.monthID == monthID);

            byte resultComponent1 = values.answerQuestion6;
            byte resultComponent2 = component2(values.answerQuestion2, values.answerQuestion5a);
            byte resultComponent3 = component3(values.answerQuestion4);
            byte resultComponent4 = component4(values.answerQuestion1, values.answerQuestion3, values.answerQuestion4);
            byte resultComponent5 = component5(answers);
            byte resultComponent6 = values.answerQuestion7;
            byte resultComponent7 = component7(values.answerQuestion8, values.answerQuestion9);

            int total = resultComponent1 + resultComponent2 + resultComponent3 + resultComponent4 +
                        resultComponent5 + resultComponent6 + resultComponent7;

            string classificationPSQI = classification(total);


            SaveResultsHabitsDto results = new SaveResultsHabitsDto
            {
                monthlyFollowUpID = answersRecentlyAdd.monthlyFollowUpID,
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

            SaveResults(results);

            var responses = RetrieveAnswers(values.accountID, values.month, values.year);

            return responses;
        }

        public RetrieveResponsesHabitsDto UpdateAnswers(UpdateResponsesHabitsDto values)
        {
            var mfuToUpdate = _bd.MFUsHabits.Find(values.monthlyFollowUpID);

            if (mfuToUpdate == null) { throw new UnstoredValuesException(); }

            mfuToUpdate.answerQuestion1 = values.answerQuestion1;
            mfuToUpdate.answerQuestion2 = values.answerQuestion2;
            mfuToUpdate.answerQuestion3 = values.answerQuestion3;
            mfuToUpdate.answerQuestion4 = values.answerQuestion4;
            mfuToUpdate.answerQuestion5a = values.answerQuestion5a;
            mfuToUpdate.answerQuestion5b = values.answerQuestion5b;
            mfuToUpdate.answerQuestion5c = values.answerQuestion5c;
            mfuToUpdate.answerQuestion5d = values.answerQuestion5d;
            mfuToUpdate.answerQuestion5e = values.answerQuestion5e;
            mfuToUpdate.answerQuestion5f = values.answerQuestion5f;
            mfuToUpdate.answerQuestion5g = values.answerQuestion5g;
            mfuToUpdate.answerQuestion5h = values.answerQuestion5h;
            mfuToUpdate.answerQuestion5i = values.answerQuestion5i;
            mfuToUpdate.answerQuestion5j = values.answerQuestion5j;
            mfuToUpdate.answerQuestion6 = values.answerQuestion6;
            mfuToUpdate.answerQuestion7 = values.answerQuestion7;
            mfuToUpdate.answerQuestion8 = values.answerQuestion8;
            mfuToUpdate.answerQuestion9 = values.answerQuestion9;

            ValidationSaveAnswers(mfuToUpdate);

            _bd.MFUsHabits.Update(mfuToUpdate);

            if (!Save()) { throw new UnstoredValuesException(); }

            byte resultComponent1 = values.answerQuestion6;
            byte resultComponent2 = component2(values.answerQuestion2, values.answerQuestion5a);
            byte resultComponent3 = component3(values.answerQuestion4);
            byte resultComponent4 = component4(values.answerQuestion1, values.answerQuestion3, values.answerQuestion4);
            byte resultComponent5 = component5(mfuToUpdate);
            byte resultComponent6 = values.answerQuestion7;
            byte resultComponent7 = component7(values.answerQuestion8, values.answerQuestion9);

            int total = resultComponent1 + resultComponent2 + resultComponent3 + resultComponent4 +
                        resultComponent5 + resultComponent6 + resultComponent7;

            string classificationPSQI = classification(total);

            var resultsToUpdate = _bd.ResultsHabits.FirstOrDefault(e => e.monthlyFollowUpID == values.monthlyFollowUpID);

            resultsToUpdate.resultComponent1 = resultComponent1;
            resultsToUpdate.resultComponent2 = resultComponent2;
            resultsToUpdate.resultComponent3 = resultComponent3;
            resultsToUpdate.resultComponent4 = resultComponent4;
            resultsToUpdate.resultComponent5 = resultComponent5;
            resultsToUpdate.resultComponent6 = resultComponent6;
            resultsToUpdate.resultComponent7 = resultComponent7;
            resultsToUpdate.globalClassification = total;
            resultsToUpdate.classification = classificationPSQI;

            ValidationSaveResults(resultsToUpdate);

            _bd.ResultsHabits.Update(resultsToUpdate);

            if (!Save()) { throw new UnstoredValuesException(); }

            var responses = RetrieveAnswers(mfuToUpdate.accountID, values.month, values.year);

            return responses;
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

        private void SaveResults(SaveResultsHabitsDto values)
        {
            HabitsResults results = new HabitsResults
            {
                monthlyFollowUpID = values.monthlyFollowUpID,
                resultComponent1 = values.resultComponent1,
                resultComponent2 = values.resultComponent2,
                resultComponent3 = values.resultComponent3,
                resultComponent4 = values.resultComponent4,
                resultComponent5 = values.resultComponent5,
                resultComponent6 = values.resultComponent6,
                resultComponent7 = values.resultComponent7,
                globalClassification = values.globalClassification,
                classification = values.classification
            };

            ValidationSaveResults(results);

            _bd.ResultsHabits.Add(results);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private void ValidationSaveAnswers(MFUsHabits mfus)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(mfus, null, null);

            if (!Validator.TryValidateObject(mfus, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }
        }

        private void ValidationSaveResults(HabitsResults results)
        {
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

            if (end < start) { end += TimeSpan.FromDays(1); }

            TimeSpan diff = end - start;

            int bedHours = (int) diff.TotalHours;

            if (bedHours == 0) { return 3; }

            float ES = ((float) response4 / bedHours) * 100;

            if (ES > 85) { return value; }

            if (ES >= 75 && ES <= 84) { value = 1; }

            if (ES >= 65 && ES <= 74) { value = 2; }

            if (ES < 65) { value = 3; }

            return value;
        }

        private static byte component5(MFUsHabits response)
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
            string value = (totalGlobal <= 5) ? "Buena calidad del sueño" : "Mala calidad del sueño";

            return value;
        }
    }
}
