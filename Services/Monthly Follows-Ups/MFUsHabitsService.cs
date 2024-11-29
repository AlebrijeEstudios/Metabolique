using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;
using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Models.Seguimientos_Mensuales.Resultados;
using AppVidaSana.Months_Dates;
using AppVidaSana.Services.IServices.IMonthly_Follow_Ups;
using AppVidaSana.ValidationValues;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services.Monthly_Follows_Ups
{
    public class MFUsHabitsService : IMFUsHabits
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;
        private readonly Months _months;
        private readonly ValidationValuesDB _validationValues;

        public MFUsHabitsService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
            _months = new Months();
            _validationValues = new ValidationValuesDB();
        }

        public async Task<RetrieveResponsesHabitsDto?> RetrieveAnswersAsync(Guid accountID, int month, int year, CancellationToken cancellationToken)
        {
            var monthStr = _months.VerifyExistMonth(month);

            RetrieveResponsesHabitsDto? responses;

            var existMonth = await _bd.Months.FirstOrDefaultAsync(e => e.month == monthStr && e.year == year, cancellationToken);

            if (existMonth is null)
            {
                responses = null;
                return responses;
            }

            var mfuHabits = await _bd.MFUsHabits.FirstOrDefaultAsync(c => c.accountID == accountID 
                                                                     && c.monthID == existMonth.monthID, cancellationToken);

            if (mfuHabits is null)
            {
                responses = null;
                return responses;
            }

            var mfuHabitsResults = await _bd.ResultsHabits.FirstOrDefaultAsync(c => c.monthlyFollowUpID == mfuHabits.monthlyFollowUpID, 
                                                                               cancellationToken);

            if (mfuHabitsResults is null)
            {
                responses = null;
                return responses;
            }

            responses = _mapper.Map<RetrieveResponsesHabitsDto>(mfuHabits);
            responses = _mapper.Map(existMonth, responses);
            _mapper.Map(mfuHabitsResults, responses);

            return responses;
        }

        public async Task<RetrieveResponsesHabitsDto?> SaveAnswersAsync(SaveResponsesHabitsDto values, CancellationToken cancellationToken)
        {
            var monthStr = _months.VerifyExistMonth(values.month);

            await ExistMonthAsync(monthStr, values.year, cancellationToken);

            var month = await _bd.Months.FirstOrDefaultAsync(e => e.month == monthStr && e.year == values.year, cancellationToken);

            if (month is null) { throw new UnstoredValuesException(); }

            var answersExisting = await _bd.MFUsHabits.AnyAsync(e => e.accountID == values.accountID 
                                                                && e.monthID == month.monthID, cancellationToken);

            if (answersExisting) { throw new RepeatRegistrationException(); }

            MFUsHabits answers = new MFUsHabits
            {
                accountID = values.accountID,
                monthID = month.monthID,
                answerQuestion1 = values.answerQuestion1,
                answerQuestion2 = values.answerQuestion2,
                answerQuestion3 = values.answerQuestion3,
                answerQuestion4 = values.answerQuestion4,
                answerQuestion5a = values.answerQuestion5a,
                answerQuestion5b = values.answerQuestion5b,
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

            _validationValues.ValidationValues(answers);

            _bd.MFUsHabits.Add(answers);

            if (!Save()) { throw new UnstoredValuesException(); }

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
                monthlyFollowUpID = answers.monthlyFollowUpID,
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

            var responses = await RetrieveAnswersAsync(values.accountID, values.month, values.year, cancellationToken);

            return responses;
        }

        public async Task<RetrieveResponsesHabitsDto?> UpdateAnswersAsync(UpdateResponsesHabitsDto values, CancellationToken cancellationToken)
        {
            var mfuToUpdate = await _bd.MFUsHabits.FindAsync(new object[] { values.monthlyFollowUpID }, cancellationToken);

            if (mfuToUpdate is null) { throw new UnstoredValuesException(); }

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

            _validationValues.ValidationValues(mfuToUpdate);

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

            var resultsToUpdate = await _bd.ResultsHabits.FirstOrDefaultAsync(e => e.monthlyFollowUpID == values.monthlyFollowUpID,
                                                                              cancellationToken);

            if (resultsToUpdate is null) { throw new UnstoredValuesException(); }

            resultsToUpdate.resultComponent1 = resultComponent1;
            resultsToUpdate.resultComponent2 = resultComponent2;
            resultsToUpdate.resultComponent3 = resultComponent3;
            resultsToUpdate.resultComponent4 = resultComponent4;
            resultsToUpdate.resultComponent5 = resultComponent5;
            resultsToUpdate.resultComponent6 = resultComponent6;
            resultsToUpdate.resultComponent7 = resultComponent7;
            resultsToUpdate.globalClassification = total;
            resultsToUpdate.classification = classificationPSQI;

            _validationValues.ValidationValues(resultsToUpdate);

            _bd.ResultsHabits.Update(resultsToUpdate);

            if (!Save()) { throw new UnstoredValuesException(); }

            var responses = await RetrieveAnswersAsync(mfuToUpdate.accountID, values.month, values.year, cancellationToken);

            return responses;
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

            _validationValues.ValidationValues(results);

            _bd.ResultsHabits.Add(results);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private async Task ExistMonthAsync(string monthStr, int year, CancellationToken cancellationToken)
        {
            var existMonth = await _bd.Months.AnyAsync(e => e.month == monthStr && e.year == year, cancellationToken);

            if (!existMonth)
            {
                MFUsMonths month = new MFUsMonths
                {
                    month = monthStr,
                    year = year
                };

                _bd.Months.Add(month);

                if (!Save()) { throw new UnstoredValuesException(); }
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

            int bedHours = (int)diff.TotalHours;

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
