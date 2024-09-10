using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Exercise_Dtos;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;
using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Models.Monthly_Follow_Ups.Results;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Services.IServices.ISeguimientos_Mensuales;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Services.Seguimientos_Mensuales
{
    public class MFUsExerciseService : IMFUsExercise
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public MFUsExerciseService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
        }

        public RetrieveResponsesExerciseDto RetrieveAnswers(Guid id, int month, int year)
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

            RetrieveResponsesExerciseDto responses;
            
            var existMonth = _bd.Months.FirstOrDefault(e => e.month == getMonth && e.year == year);

            if(existMonth == null)
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

            var answersExisting = _bd.MFUsExercise.Any(e => e.accountID == values.accountID && e.monthID == monthID);

            if (answersExisting) { throw new RepeatRegistrationException(); }

            var accountExisting = _bd.Accounts.Find(values.accountID);

            if (accountExisting == null) { throw new UserNotFoundException(); }

            string LevelAF = "BAJO";

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

            if (levelHigh)
            {
                LevelAF = "ALTO";
            }
            else
            {
                if (levelModerate)
                {
                    LevelAF = "MODERADO";
                }
            }

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

            string LevelAF = "BAJO";

            float METactvigorous = actVigorous(values.question1, values.question2);
            float METactmoderate = actModerate(values.question3, values.question4);
            float METactwalking = actWalking(values.question5, values.question6);
            float TotalMET = totalMET(METactvigorous, METactmoderate, METactwalking);

            string sedentary = sedentaryBehavior(values.question7);

            bool levelHigh = levelActHigh(values.question1, METactvigorous, METactmoderate, METactwalking);
            bool levelModerate = levelActModerate(answers, METactvigorous, METactmoderate, METactwalking);

            if (levelHigh)
            {
                LevelAF = "ALTO";
            }
            else
            {
                if (levelModerate)
                {
                    LevelAF = "MODERADO";
                }
            }

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
            string result = "AUSENTE";

            if(res7 > 6)
            {
                result = "PRESENTE";
            }

            return result;
        }

        private static bool levelActHigh(int res1, float MET_AFvigorous, float MET_AFmoderate, float MET_AFwalking)
        {
            bool criterion1 = false;
            bool criterion2 = false;

            if (res1 >= 3 && (int) MET_AFvigorous >= 1500)
            {
                criterion1 = true;
            }

            if ((int) (MET_AFwalking + MET_AFmoderate + MET_AFvigorous) >= 3000)
            {
                criterion2 = true;
            }

            return criterion1 || criterion2;
        }

        private static bool levelActModerate(AnswersDto answers, float MET_AFvigorous, float MET_AFmoderate, float MET_AFwalking)
        {
            bool criterion1 = false;
            bool criterion2 = false;
            bool criterion3 = false;

            if (answers.question1 >= 3 && answers.question2 >= 20)
            {
                criterion1 = true;
            }

            if ((answers.question3 >= 5 && answers.question4 >= 30) || (answers.question5 >= 5 && answers.question6 >=30))
            {
                criterion2 = true;
            }

            if( ((answers.question1 + answers.question5) >= 5 && (int) (MET_AFvigorous + MET_AFwalking) >= 600) || ((answers.question3 + answers.question5) >= 5 && (int) (MET_AFmoderate + MET_AFwalking) >= 600))
            {
                criterion3 = true;
            }

            return criterion1 || criterion2 || criterion3;

        }
    }
}
