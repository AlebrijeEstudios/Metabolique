using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Exercise_Dtos;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;
using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Models.Monthly_Follow_Ups.Results;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Months_Dates;
using AppVidaSana.Services.IServices.ISeguimientos_Mensuales;
using AutoMapper;
using System.ComponentModel.DataAnnotations;
using System.Threading;

namespace AppVidaSana.Services.Seguimientos_Mensuales
{
    public class MFUsExerciseService : IMFUsExercise
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;
        private Months _months;

        public MFUsExerciseService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
            _months = new Months();
        }

        public RetrieveResponsesExerciseDto RetrieveAnswers(Guid id, int month, int year)
        {
            var monthStr = _months.VerifyExistMonth(month);

            RetrieveResponsesExerciseDto responses;

            var existMonth = _bd.Months.FirstOrDefault(e => e.month == monthStr && e.year == year);

            if (existMonth == null)
            {
                responses = null;
                return responses;
            }

            var mfuExercise = _bd.MFUsExercise.FirstOrDefault(c => c.accountID == id && c.monthID == existMonth.monthID);

            if (mfuExercise == null)
            {
                responses = null;
                return responses;
            }

            var mfuExerciseResults = _bd.ResultsExercise.FirstOrDefault(c => c.monthlyFollowUpID == mfuExercise.monthlyFollowUpID);

            if (mfuExerciseResults == null)
            {
                responses = null;
                return responses;
            }

            responses = _mapper.Map<RetrieveResponsesExerciseDto>(mfuExercise);
            responses = _mapper.Map(existMonth, responses);
            _mapper.Map(mfuExerciseResults, responses);

            return responses;
        }

        public RetrieveResponsesExerciseDto SaveAnswers(SaveResponsesExerciseDto values)
        {
            var monthStr = _months.VerifyExistMonth(values.month);

            ExistMonth(monthStr, values.year);

            Guid monthID = _bd.Months.FirstOrDefault(e => e.month == monthStr && e.year == values.year).monthID;

            var answersExisting = _bd.MFUsExercise.Any(e => e.accountID == values.accountID && e.monthID == monthID);

            if (answersExisting) { throw new RepeatRegistrationException(); }

            var accountExisting = _bd.Accounts.Find(values.accountID);

            if (accountExisting == null) { throw new UserNotFoundException(); }

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

            LevelAF = (levelHigh && levelModerate || levelHigh && !levelModerate) ? "ALTO" : "MODERADO";

            MFUsExercise mfus = new MFUsExercise
            {
                accountID = values.accountID,
                monthID = monthID,
                question1 = values.question1,
                question2 = values.question2,
                question3 = values.question3,
                question4 = values.question4,
                question5 = values.question5,
                question6 = values.question6,
                question7 = values.question7
            };

            ValidationSaveAnswers(mfus);

            _bd.MFUsExercise.Add(mfus);

            if (!Save()) { throw new UnstoredValuesException(); }

            Guid monthlyFollowUpID = _bd.MFUsExercise.FirstOrDefault(e => e.monthID == monthID
                                                                     && e.accountID == values.accountID).monthlyFollowUpID;

            SaveResultsExerciseDto results = new SaveResultsExerciseDto
            {
                monthlyFollowUpID = monthlyFollowUpID,
                actWalking = METactwalking,
                actModerate = METactmoderate,
                actVigorous = METactvigorous,
                totalMET = TotalMET,
                sedentaryBehavior = sedentary,
                levelAF = LevelAF
            };

            SaveResults(results);

            var responses = RetrieveAnswers(values.accountID, values.month, values.year);

            return responses;
        }


        public RetrieveResponsesExerciseDto UpdateAnswers(UpdateResponsesExerciseDto values)
        {
            var mfuToUpdate = _bd.MFUsExercise.Find(values.monthlyFollowUpID);

            if (mfuToUpdate == null) { throw new UnstoredValuesException(); }

            mfuToUpdate.question1 = values.question1;
            mfuToUpdate.question2 = values.question2;
            mfuToUpdate.question3 = values.question3;
            mfuToUpdate.question4 = values.question4;
            mfuToUpdate.question5 = values.question5;
            mfuToUpdate.question6 = values.question6;
            mfuToUpdate.question7 = values.question7;

            ValidationSaveAnswers(mfuToUpdate);

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

            LevelAF = (levelHigh && levelModerate || levelHigh && !levelModerate) ? "ALTO" : "MODERADO";

            var resultsToUpdate = _bd.ResultsExercise.FirstOrDefault(e => e.monthlyFollowUpID == values.monthlyFollowUpID);

            resultsToUpdate.actWalking = METactwalking;
            resultsToUpdate.actModerate = METactmoderate;
            resultsToUpdate.actVigorous = METactmoderate;
            resultsToUpdate.totalMET = TotalMET;
            resultsToUpdate.sedentaryBehavior = sedentary;
            resultsToUpdate.levelAF = LevelAF;

            ValidationSaveResults(resultsToUpdate);

            _bd.ResultsExercise.Update(resultsToUpdate);

            if (!Save()) { throw new UnstoredValuesException(); }

            var responses = RetrieveAnswers(mfuToUpdate.accountID, values.month, values.year);

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

            ValidationSaveResults(results);

            _bd.ResultsExercise.Add(results);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private void ExistMonth(string monthStr, int year)
        {
            var existMonth = _bd.Months.Any(e => e.month == monthStr && e.year == year);

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

        private void ValidationSaveAnswers(MFUsExercise mfus)
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

        private void ValidationSaveResults(ExerciseResults results)
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

        private static float actVigorous(int res1, int res2)
        {
            return (float) 8.0 * res2 * res1;
        }

        private static float actModerate(int res3, int res4)
        {
            return (float) 4.0 * res4 * res3;
        }

        private static float actWalking(int res5, int res6)
        {
            return (float) 3.3 * res6 * res5;
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
