using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Habits;
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

            RetrieveResponsesExerciseDto response;
            
            var monthRecord = _bd.Months.FirstOrDefault(e => e.month == existDate && e.year == year);

            if(monthRecord == null)
            {
                response = new RetrieveResponsesExerciseDto();
                return response;
            }

            var records = _bd.MFUsExercise.FirstOrDefault(c => c.accountID == id && c.monthID == monthRecord.monthID);

            if (records == null)
            {
                response = new RetrieveResponsesExerciseDto();
                return response;
            }

            var results = _bd.ResultsExercise.FirstOrDefault(c => c.monthlyFollowUpID == records.monthlyFollowUpID);

            if (results == null)
            {
                response = new RetrieveResponsesExerciseDto();
                return response;
            }

            response = _mapper.Map<RetrieveResponsesExerciseDto>(records);
            response = _mapper.Map(monthRecord, response);
            _mapper.Map(results, response);

            return response;
        }

        public SaveResultsExerciseDto SaveAnswers(SaveResponsesExerciseDto res)
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

            var answersExisting = _bd.MFUsExercise.Any(e => e.accountID == res.accountID &&
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

            string LevelAF = "BAJO";

            RetrieveResponsesExerciseDto response = new RetrieveResponsesExerciseDto
            {
                question1 = res.question1,
                question2 = res.question2,
                question3 = res.question3,
                question4 = res.question4,
                question5 = res.question5,
                question6 = res.question6,
                question7 = res.question7
            }; 

            float METactvigorous = actVigorous(res.question1, res.question2);
            float METactmoderate = actModerate(res.question3, res.question4);
            float METactwalking = actWalking(res.question5, res.question6);
            float TotalMET = totalMET(METactvigorous, METactmoderate, METactwalking);

            string sedentary = sedentaryBehavior(res.question7);

            bool levelHigh = levelActHigh(res.question1, METactvigorous, METactmoderate, METactwalking);
            bool levelModerate = levelActModerate(response, METactvigorous, METactmoderate, METactwalking);

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
                accountID = res.accountID,
                monthID = monthID,
                question1 = res.question1,
                question2 = res.question2,
                question3 = res.question3,
                question4 = res.question4,
                question5 = res.question5,
                question6 = res.question6,
                question7 = res.question7
            };

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

            _bd.MFUsExercise.Add(mfus);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            SaveResultsExerciseDto results = new SaveResultsExerciseDto
            {
                accountID = res.accountID,
                month = responseMapping[res.month],
                year = res.year,
                actWalking = METactwalking,
                actModerate = METactmoderate,
                actVigorous = METactvigorous,
                totalMET = TotalMET,
                sedentaryBehavior = sedentary,
                levelAF = LevelAF
            };

            return results;
        }

        public string SaveResults(SaveResultsExerciseDto res)
        {

            Guid monthID = _bd.Months.FirstOrDefault(e => e.month == res.month && e.year == res.year).monthID;

            var mfusHabit = _bd.MFUsExercise.FirstOrDefault(c => c.accountID == res.accountID
                            && c.monthID == monthID);

            if (mfusHabit == null)
            {
                throw new HabitNotFoundException();
            }

            ExerciseResults results = new ExerciseResults
            {
                monthlyFollowUpID = mfusHabit.monthlyFollowUpID,
                actWalking = res.actWalking,
                actModerate = res.actModerate,
                actVigorous = res.actVigorous,
                totalMET = res.totalMET,
                sedentaryBehavior = res.sedentaryBehavior,
                levelAF = res.levelAF
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

            _bd.ResultsExercise.Add(results);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return "Sus respuestas han sido guardadas correctamente";
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

        private static bool levelActModerate(RetrieveResponsesExerciseDto res, float MET_AFvigorous, float MET_AFmoderate, float MET_AFwalking)
        {
            bool criterion1 = false;
            bool criterion2 = false;
            bool criterion3 = false;

            if (res.question1 >= 3 && res.question2 >= 20)
            {
                criterion1 = true;
            }

            if ((res.question3 >= 5 && res.question4 >= 30) || (res.question5 >= 5 && res.question6 >=30))
            {
                criterion2 = true;
            }

            if( ((res.question1 + res.question5) >= 5 && (int) (MET_AFvigorous + MET_AFwalking) >= 600) || ((res.question3 + res.question5) >= 5 && (int) (MET_AFmoderate + MET_AFwalking) >= 600))
            {
                criterion3 = true;
            }

            return criterion1 || criterion2 || criterion3;

        }
    }
}
