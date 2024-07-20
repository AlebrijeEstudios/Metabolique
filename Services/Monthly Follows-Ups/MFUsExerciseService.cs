using AppVidaSana.Data;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;
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

        public string SaveAnswers(SaveResponsesDto res)
        {
            var count = _bd.Accounts.Find(res.accountID);

            if (count == null)
            {
                return "Algo salio mal, intentelo de nuevo";
            }

            string LevelAF = "BAJO";

            RetrieveResponsesDto response = new RetrieveResponsesDto
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
                month = res.month,
                year = res.year,
                question1 = res.question1,
                question2 = res.question2,
                question3 = res.question3,
                question4 = res.question4,
                question5 = res.question5,
                question6 = res.question6,
                question7 = res.question7,
                actWalking = METactwalking,
                actModerate = METactmoderate,
                actVigorous = METactvigorous,
                totalMET = TotalMET,
                sedentaryBehavior = sedentary,
                levelAF = LevelAF,
                account = null
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

            _bd.MFUsExcercise.Add(mfus);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return "Sus respuestas han sido guardadas correctamente";
        }

        public RetrieveResponsesDto RetrieveAnswers(Guid id, string month, int year)
        {
            var reg = _bd.MFUsExcercise.FirstOrDefault(c => c.accountID == id && c.month == month && c.year == year);

            if (reg == null)
            {
                throw new UserNotFoundException();
            }

            RetrieveResponsesDto res = _mapper.Map<RetrieveResponsesDto>(reg);

            return res;
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

            if ((int) (MET_AFwalking + MET_AFmoderate) >= 3000)
            {
                criterion2 = true;
            }

            if ((int)(MET_AFwalking + MET_AFvigorous) >= 3000)
            {
                criterion2 = true;
            }

            return criterion1 || criterion2;
        }

        private static bool levelActModerate(RetrieveResponsesDto res, float MET_AFvigorous, float MET_AFmoderate, float MET_AFwalking)
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
