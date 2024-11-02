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
        private Months _months;
        private ValidationValuesDB _validationValues;

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

        public async Task<ResultsMFUsFoodDto?> SaveAnswersAsync(AnswersMFUsFoodDto values, CancellationToken cancellationToken)
        {
            var monthStr = _months.VerifyExistMonth(values.month);

            await ExistMonth(monthStr, values.year, cancellationToken);

            var month = await _bd.Months.FirstOrDefaultAsync(e => e.month == monthStr && e.year == values.year, cancellationToken);

            var answersExisting = await _bd.MFUsExercise.AnyAsync(e => e.accountID == values.accountID && e.monthID == month.monthID, cancellationToken);

            if (answersExisting) { throw new RepeatRegistrationException(); }

            MFUsFood mfus = new MFUsFood
            {
                accountID = values.accountID,
                monthID = month.monthID,
                answerQuestion1 = values.question1,
                answerQuestion2 = values.question2,
                answerQuestion3 = values.question3,
                answerQuestion4 = values.question4,
                answerQuestion5 = values.question5,
                answerQuestion6 = values.question6,
                answerQuestion7 = values.question7,
                answerQuestion8 = values.question8,
                answerQuestion9 = values.question9
            };

            _validationValues.ValidationValues(mfus);

            _bd.MFUsFood.Add(mfus);

            if (!Save()) { throw new UnstoredValuesException(); }

            var totalPts = TotalPts(values);
            var classification = Classification(totalPts);

            FoodResults results = new FoodResults
            {
                monthlyFollowUpID = mfus.monthlyFollowUpID,
                totalPts = totalPts,
                classification = classification
            };

            _validationValues.ValidationValues(results);

            _bd.ResultsFood.Add(results);

            if (!Save()) { throw new UnstoredValuesException(); }

            var responses = await RetrieveAnswersAsync(values.accountID, values.month, values.year, cancellationToken);

            return responses;
        }

        public ResultsMFUsFoodDto UpdateAnswers(UpdateAnswersMFUsFoodDto values)
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

        private async Task ExistMonth(string monthStr, int year, CancellationToken cancellationToken)
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

        private float TotalPts(AnswersMFUsFoodDto values)
        {
            float total = values.question1 + values.question2 + values.question3 + values.question4 + values.question5 +
                          values.question6 + values.question7 + values.question8 + values.question9;

            if (values.question1 == 10) { total = total + 2; }
            if (values.question2 == 10) { total = total + 2; }
            if (values.question3 == 10) { total = total + 2; }
            if (values.question4 == 10) { total = total + 2; }

            if (values.question5 == 10) { total = total + 1; }
            if (values.question6 == 10) { total = total + 1; }

            return total;
        }

        private string Classification(float totalPts)
        {
            string classif = "";

            if (totalPts < 50) { classif = "Poco saludable"; }
            if (totalPts >= 50 && totalPts <= 80) { classif = "Necesita cambios"; }
            if (totalPts > 80) { classif = "Saludable"; }

            return classif;
        }

    }
}
