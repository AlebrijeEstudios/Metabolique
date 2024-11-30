using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Exercise_Dtos;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;
using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Models.Monthly_Follow_Ups.Results;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Months_Dates;
using AppVidaSana.Services.IServices.ISeguimientos_Mensuales;
using AppVidaSana.ValidationValues;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services.Seguimientos_Mensuales
{
    public class MFUsExerciseService : IMFUsExercise
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;
        private readonly Months _months;
        private readonly ValidationValuesDB _validationValues;

        public MFUsExerciseService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
            _months = new Months();
            _validationValues = new ValidationValuesDB();
        }

        public async Task<RetrieveResponsesExerciseDto?> RetrieveAnswersAsync(Guid accountID, int month, int year, CancellationToken cancellationToken)
        {
            var monthStr = _months.VerifyExistMonth(month);

            RetrieveResponsesExerciseDto? responses;

            var existMonth = await _bd.Months.FirstOrDefaultAsync(e => e.month == monthStr && e.year == year, cancellationToken);

            if (existMonth is null)
            {
                responses = null;
                return responses;
            }

            var mfuExercise = await _bd.MFUsExercise.FirstOrDefaultAsync(c => c.accountID == accountID
                                                                         && c.monthID == existMonth.monthID, cancellationToken);

            if (mfuExercise is null)
            {
                responses = null;
                return responses;
            }

            var mfuExerciseResults = await _bd.ResultsExercise.FirstOrDefaultAsync(c => c.monthlyFollowUpID == mfuExercise.monthlyFollowUpID,
                                                                                   cancellationToken);

            if (mfuExerciseResults is null)
            {
                responses = null;
                return responses;
            }

            responses = _mapper.Map<RetrieveResponsesExerciseDto>(mfuExercise);
            responses = _mapper.Map(existMonth, responses);
            _mapper.Map(mfuExerciseResults, responses);

            return responses;
        }

        public async Task<RetrieveResponsesExerciseDto?> SaveAnswersAsync(SaveResponsesExerciseDto values, CancellationToken cancellationToken)
        {
            var monthStr = _months.VerifyExistMonth(values.month);

            await ExistMonthAsync(monthStr, values.year, cancellationToken);

            var month = await _bd.Months.FirstOrDefaultAsync(e => e.month == monthStr
                                                             && e.year == values.year, cancellationToken);

            if (month is null) { throw new UnstoredValuesException(); }

            var answersExisting = await _bd.MFUsExercise.AnyAsync(e => e.accountID == values.accountID
                                                                  && e.monthID == month.monthID, cancellationToken);

            if (answersExisting) { throw new RepeatRegistrationException(); }

            AnswersDto answers = new AnswersDto
            {
                question1 = values.question1,
                question2 = values.question2,
                question3 = values.question3,
                question4 = values.question4,
                question5 = values.question5,
                question6 = values.question6,
                question7 = values.question7
            };

            float METactvigorous = actVigorous(values.question1, values.question2);
            float METactmoderate = actModerate(values.question3, values.question4);
            float METactwalking = actWalking(values.question5, values.question6);
            float TotalMET = totalMET(METactvigorous, METactmoderate, METactwalking);

            string sedentary = sedentaryBehavior(values.question7);

            bool levelHigh = levelActHigh(values.question1, METactvigorous, METactmoderate, METactwalking);
            bool levelModerate = levelActModerate(answers, METactvigorous, METactmoderate, METactwalking);

            string LevelAF = "BAJO";

            LevelAF = levelHigh ? "ALTO" : "MODERADO";

            MFUsExercise mfus = new MFUsExercise
            {
                accountID = values.accountID,
                monthID = month.monthID,
                question1 = values.question1,
                question2 = values.question2,
                question3 = values.question3,
                question4 = values.question4,
                question5 = values.question5,
                question6 = values.question6,
                question7 = values.question7
            };

            _validationValues.ValidationValues(mfus);

            _bd.MFUsExercise.Add(mfus);

            if (!Save()) { throw new UnstoredValuesException(); }

            SaveResultsExerciseDto results = new SaveResultsExerciseDto
            {
                monthlyFollowUpID = mfus.monthlyFollowUpID,
                actWalking = METactwalking,
                actModerate = METactmoderate,
                actVigorous = METactvigorous,
                totalMET = TotalMET,
                sedentaryBehavior = sedentary,
                levelAF = LevelAF
            };

            SaveResults(results);

            var responses = await RetrieveAnswersAsync(values.accountID, values.month, values.year, cancellationToken);

            return responses;
        }


        public async Task<RetrieveResponsesExerciseDto?> UpdateAnswersAsync(UpdateResponsesExerciseDto values, CancellationToken cancellationToken)
        {
            var mfuToUpdate = await _bd.MFUsExercise.FindAsync(new object[] { values.monthlyFollowUpID }, cancellationToken);

            if (mfuToUpdate is null) { throw new UnstoredValuesException(); }

            mfuToUpdate.question1 = values.question1;
            mfuToUpdate.question2 = values.question2;
            mfuToUpdate.question3 = values.question3;
            mfuToUpdate.question4 = values.question4;
            mfuToUpdate.question5 = values.question5;
            mfuToUpdate.question6 = values.question6;
            mfuToUpdate.question7 = values.question7;

            _validationValues.ValidationValues(mfuToUpdate);

            _bd.MFUsExercise.Update(mfuToUpdate);

            if (!Save()) { throw new UnstoredValuesException(); }

            AnswersDto answers = new AnswersDto
            {
                question1 = values.question1,
                question2 = values.question2,
                question3 = values.question3,
                question4 = values.question4,
                question5 = values.question5,
                question6 = values.question6,
                question7 = values.question7
            };

            float METactvigorous = actVigorous(values.question1, values.question2);
            float METactmoderate = actModerate(values.question3, values.question4);
            float METactwalking = actWalking(values.question5, values.question6);
            float TotalMET = totalMET(METactvigorous, METactmoderate, METactwalking);

            string sedentary = sedentaryBehavior(values.question7);

            bool levelHigh = levelActHigh(values.question1, METactvigorous, METactmoderate, METactwalking);
            bool levelModerate = levelActModerate(answers, METactvigorous, METactmoderate, METactwalking);

            string LevelAF = "BAJO";

            LevelAF = levelHigh ? "ALTO" : "MODERADO";

            var resultsToUpdate = await _bd.ResultsExercise.FirstOrDefaultAsync(e => e.monthlyFollowUpID == values.monthlyFollowUpID,
                                                                                cancellationToken);

            if (resultsToUpdate is null) { throw new UnstoredValuesException(); }

            resultsToUpdate.actWalking = METactwalking;
            resultsToUpdate.actModerate = METactmoderate;
            resultsToUpdate.actVigorous = METactmoderate;
            resultsToUpdate.totalMET = TotalMET;
            resultsToUpdate.sedentaryBehavior = sedentary;
            resultsToUpdate.levelAF = LevelAF;

            _validationValues.ValidationValues(resultsToUpdate);

            _bd.ResultsExercise.Update(resultsToUpdate);

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

        private void SaveResults(SaveResultsExerciseDto values)
        {
            ExerciseResults results = new ExerciseResults
            {
                monthlyFollowUpID = values.monthlyFollowUpID,
                actWalking = values.actWalking,
                actModerate = values.actModerate,
                actVigorous = values.actVigorous,
                totalMET = values.totalMET,
                sedentaryBehavior = values.sedentaryBehavior,
                levelAF = values.levelAF
            };

            _validationValues.ValidationValues(results);

            _bd.ResultsExercise.Add(results);

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

        private static float actVigorous(int res1, int res2)
        {
            return (float)8.0 * res2 * res1;
        }

        private static float actModerate(int res3, int res4)
        {
            return (float)4.0 * res4 * res3;
        }

        private static float actWalking(int res5, int res6)
        {
            return (float)3.3 * res6 * res5;
        }

        private static float totalMET(float met1, float met2, float met3)
        {
            return met1 + met2 + met3;
        }

        private static string sedentaryBehavior(int res7)
        {
            var result = (res7 > 6) ? "PRESENTE" : "AUSENTE";

            return result;
        }

        private static bool levelActHigh(int res1, float MET_AFvigorous, float MET_AFmoderate, float MET_AFwalking)
        {
            bool criterion1, criterion2 = false;

            criterion1 = (res1 >= 3 && MET_AFvigorous >= 1500);

            criterion2 = (MET_AFwalking + MET_AFmoderate >= 3000) || (MET_AFwalking + MET_AFvigorous >= 3000);

            return criterion1 || criterion2;
        }

        private static bool levelActModerate(AnswersDto answers, float MET_AFvigorous, float MET_AFmoderate, float MET_AFwalking)
        {
            bool criterion1, criterion2, criterion3 = false;

            criterion1 = (answers.question1 >= 3 && answers.question2 >= 20);

            criterion2 = (answers.question3 >= 5 && answers.question4 >= 30) || (answers.question5 >= 5 && answers.question6 >= 30);

            criterion3 = ((answers.question1 + answers.question5) >= 5 && (MET_AFvigorous + MET_AFwalking) >= 600) || ((answers.question3 + answers.question5) >= 5 && (MET_AFmoderate + MET_AFwalking) >= 600);

            return criterion1 || criterion2 || criterion3;

        }
    }
}
