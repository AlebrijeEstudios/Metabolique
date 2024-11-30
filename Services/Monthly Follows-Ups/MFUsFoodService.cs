using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Food_Dtos;
using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Models.Monthly_Follow_Ups.Results;
using AppVidaSana.Months_Dates;
using AppVidaSana.Services.IServices.IMonthly_Follow_Ups;
using AppVidaSana.ValidationValues;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services.Monthly_Follows_Ups
{
    public class MFUsFoodService : IMFUsFood
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;
        private readonly Months _months;
        private readonly ValidationValuesDB _validationValues;

        public MFUsFoodService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
            _months = new Months();
            _validationValues = new ValidationValuesDB();
        }

        public async Task<ResultsMFUsFoodDto?> RetrieveAnswersAsync(Guid accountID, int month, int year, CancellationToken cancellationToken)
        {
            var monthStr = _months.VerifyExistMonth(month);

            ResultsMFUsFoodDto? results;

            var existMonth = await _bd.Months.FirstOrDefaultAsync(e => e.month == monthStr && e.year == year, cancellationToken);

            if (existMonth is null)
            {
                results = null;
                return results;
            }

            var mfuFood = await _bd.MFUsFood.FirstOrDefaultAsync(c => c.accountID == accountID
                                                                 && c.monthID == existMonth.monthID, cancellationToken);

            if (mfuFood is null)
            {
                results = null;
                return results;
            }

            var mfuFoodResults = await _bd.ResultsFood.FirstOrDefaultAsync(c => c.monthlyFollowUpID == mfuFood.monthlyFollowUpID, cancellationToken);

            if (mfuFoodResults is null)
            {
                results = null;
                return results;
            }

            results = _mapper.Map<ResultsMFUsFoodDto>(mfuFood);
            results = _mapper.Map(existMonth, results);
            _mapper.Map(mfuFoodResults, results);

            return results;
        }

        public async Task<ResultsMFUsFoodDto?> SaveAnswersAsync(MFUsFoodDto values, CancellationToken cancellationToken)
        {
            var monthStr = _months.VerifyExistMonth(values.month);

            await ExistMonthAsync(monthStr, values.year, cancellationToken);

            var month = await _bd.Months.FirstOrDefaultAsync(e => e.month == monthStr
                                                             && e.year == values.year, cancellationToken);

            if (month is null) { throw new UnstoredValuesException(); }

            var answersExisting = await _bd.MFUsFood.AnyAsync(e => e.accountID == values.accountID
                                                              && e.monthID == month.monthID, cancellationToken);

            if (answersExisting) { throw new RepeatRegistrationException(); }

            MFUsFood mfus = new MFUsFood
            {
                accountID = values.accountID,
                monthID = month.monthID,
                answerQuestion1 = values.answerQuestion1,
                answerQuestion2 = values.answerQuestion2,
                answerQuestion3 = values.answerQuestion3,
                answerQuestion4 = values.answerQuestion4,
                answerQuestion5 = values.answerQuestion5,
                answerQuestion6 = values.answerQuestion6,
                answerQuestion7 = values.answerQuestion7,
                answerQuestion8 = values.answerQuestion8,
                answerQuestion9 = values.answerQuestion9
            };

            _validationValues.ValidationValues(mfus);

            _bd.MFUsFood.Add(mfus);

            if (!Save()) { throw new UnstoredValuesException(); }

            AnswersMFUsFoodDto answers = new AnswersMFUsFoodDto
            {
                answerQuestion1 = values.answerQuestion1,
                answerQuestion2 = values.answerQuestion2,
                answerQuestion3 = values.answerQuestion3,
                answerQuestion4 = values.answerQuestion4,
                answerQuestion5 = values.answerQuestion5,
                answerQuestion6 = values.answerQuestion6,
                answerQuestion7 = values.answerQuestion7,
                answerQuestion8 = values.answerQuestion8,
                answerQuestion9 = values.answerQuestion9
            };

            SaveResults(mfus.monthlyFollowUpID, answers);

            var responses = await RetrieveAnswersAsync(values.accountID, values.month, values.year, cancellationToken);

            return responses;
        }

        public async Task<ResultsMFUsFoodDto?> UpdateAnswersAsync(UpdateAnswersMFUsFoodDto values, CancellationToken cancellationToken)
        {
            var mfuToUpdate = await _bd.MFUsFood.FindAsync(new object[] { values.monthlyFollowUpID }, cancellationToken);

            if (mfuToUpdate is null) { throw new UnstoredValuesException(); }

            mfuToUpdate.answerQuestion1 = values.answerQuestion1;
            mfuToUpdate.answerQuestion2 = values.answerQuestion2;
            mfuToUpdate.answerQuestion3 = values.answerQuestion3;
            mfuToUpdate.answerQuestion4 = values.answerQuestion4;
            mfuToUpdate.answerQuestion5 = values.answerQuestion5;
            mfuToUpdate.answerQuestion6 = values.answerQuestion6;
            mfuToUpdate.answerQuestion7 = values.answerQuestion7;
            mfuToUpdate.answerQuestion8 = values.answerQuestion8;
            mfuToUpdate.answerQuestion9 = values.answerQuestion9;

            _validationValues.ValidationValues(mfuToUpdate);

            _bd.MFUsFood.Update(mfuToUpdate);

            if (!Save()) { throw new UnstoredValuesException(); }

            AnswersMFUsFoodDto answers = new AnswersMFUsFoodDto
            {
                answerQuestion1 = values.answerQuestion1,
                answerQuestion2 = values.answerQuestion2,
                answerQuestion3 = values.answerQuestion3,
                answerQuestion4 = values.answerQuestion4,
                answerQuestion5 = values.answerQuestion5,
                answerQuestion6 = values.answerQuestion6,
                answerQuestion7 = values.answerQuestion7,
                answerQuestion8 = values.answerQuestion8,
                answerQuestion9 = values.answerQuestion9
            };

            await UpdateResultsAsync(values.monthlyFollowUpID, answers, cancellationToken);

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

        private void SaveResults(Guid monthlyFollowUpID, AnswersMFUsFoodDto answers)
        {
            var totalPts = TotalPts(answers);
            var classification = Classification(totalPts);

            FoodResults results = new FoodResults
            {
                monthlyFollowUpID = monthlyFollowUpID,
                totalPts = totalPts,
                classification = classification
            };

            _validationValues.ValidationValues(results);

            _bd.ResultsFood.Add(results);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private async Task UpdateResultsAsync(Guid monthlyFollowUpID, AnswersMFUsFoodDto answers, CancellationToken cancellationToken)
        {
            var totalPts = TotalPts(answers);
            var classification = Classification(totalPts);

            var resultsToUpdate = await _bd.ResultsFood.FirstOrDefaultAsync(e => e.monthlyFollowUpID == monthlyFollowUpID,
                                                                            cancellationToken);

            if (resultsToUpdate is null) { throw new UnstoredValuesException(); }

            resultsToUpdate.totalPts = totalPts;
            resultsToUpdate.classification = classification;

            _validationValues.ValidationValues(resultsToUpdate);

            _bd.ResultsFood.Update(resultsToUpdate);

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

        private static float TotalPts(AnswersMFUsFoodDto values)
        {
            float tolerancia = 0.0001f;

            float total = values.answerQuestion1 + values.answerQuestion2 + values.answerQuestion3 + values.answerQuestion4 +
                          values.answerQuestion5 + values.answerQuestion6 + values.answerQuestion7 + values.answerQuestion8 +
                          values.answerQuestion9;

            if (Math.Abs(values.answerQuestion1 - 10) < tolerancia) { total = total + 2; }
            if (Math.Abs(values.answerQuestion2 - 10) < tolerancia) { total = total + 2; }
            if (Math.Abs(values.answerQuestion3 - 10) < tolerancia) { total = total + 2; }
            if (Math.Abs(values.answerQuestion4 - 10) < tolerancia) { total = total + 2; }

            if (Math.Abs(values.answerQuestion5 - 10) < tolerancia) { total = total + 1; }
            if (Math.Abs(values.answerQuestion6 - 10) < tolerancia) { total = total + 1; }

            return total;
        }

        private static string Classification(float totalPts)
        {
            string classif = "";

            if (totalPts < 50) { classif = "Poco saludable"; }
            if (totalPts >= 50 && totalPts <= 80) { classif = "Necesita cambios"; }
            if (totalPts > 80) { classif = "Saludable"; }

            return classif;
        }
    }
}