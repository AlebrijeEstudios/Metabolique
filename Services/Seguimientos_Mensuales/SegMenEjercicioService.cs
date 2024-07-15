using AppVidaSana.Data;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Services.IServices.ISeguimientos_Mensuales;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Services.Seguimientos_Mensuales
{
    public class SegMenEjercicioService : ISegMenEjercicio
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public SegMenEjercicioService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;

        }

        public string GuardarRespuestas(GuardarRespuestasDto res)
        {
            var count = _bd.Cuentas.Find(res.id);

            if (count == null)
            {
                return "Algo salio mal, intentelo de nuevo";
            }

            string NivelAF = "BAJO";

            RecuperarRespuestasDto resp = new RecuperarRespuestasDto
            {
                pregunta1 = res.pregunta1,
                pregunta2 = res.pregunta2,
                pregunta3 = res.pregunta3,
                pregunta4 = res.pregunta4,
                pregunta5 = res.pregunta5,
                pregunta6 = res.pregunta6,
                pregunta7 = res.pregunta7
            }; 

            float METactvi = actvigorosa(res.pregunta1, res.pregunta2);
            float METactmod = actmoderada(res.pregunta3, res.pregunta4);
            float METactcam = actcaminata(res.pregunta5, res.pregunta6);
            float TotalMET = totalMET(METactvi, METactmod, METactcam);

            string conducSend = conductasendentaria(res.pregunta7);

            bool nivelAlto = nivelAFAlto(res.pregunta1, METactvi, METactmod, METactcam);
            bool nivelModerado = nivelAFModerado(resp, METactvi, METactmod, METactcam);

            if (nivelAlto)
            {
                NivelAF = "ALTO";
            }
            else
            {
                if (nivelModerado)
                {
                    NivelAF = "MODERADO";
                }
            }

            SegMenEjercicio sme = new SegMenEjercicio
            {
                cuentaID = res.id,
                mes = res.mes,
                año = res.año,
                pregunta1 = res.pregunta1,
                pregunta2 = res.pregunta2,
                pregunta3 = res.pregunta3,
                pregunta4 = res.pregunta4,
                pregunta5 = res.pregunta5,
                pregunta6 = res.pregunta6,
                pregunta7 = res.pregunta7,
                actCaminata = METactcam,
                actfModerada = METactmod,
                actfVigorosa = METactvi,
                totalMET = TotalMET,
                conductaSend = conducSend,
                nivelAF = NivelAF,
                cuenta = null
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(sme, null, null);

            if (!Validator.TryValidateObject(sme, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }

            _bd.SegMenEjercicios.Add(sme);
            Guardar();
            return "Sus respuestas han sido guardadas correctamente";
        }

        public RecuperarRespuestasDto RecuperarRespuestas(Guid id, string mes, int año)
        {
            var reg = _bd.SegMenEjercicios.FirstOrDefault(c => c.cuentaID == id && c.mes == mes && c.año == año);

            if (reg == null)
            {
                throw new UserNotFoundException();
            }

            RecuperarRespuestasDto res = _mapper.Map<RecuperarRespuestasDto>(reg);

            return res;
        }

        public bool Guardar()
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

        private static float actvigorosa(int res1, int res2)
        {
            return (float) 8.0 * res2 * res1;
        }

        private static float actmoderada(int res3, int res4)
        {
            return (float) 4.0 * res4 * res3;
        }
        
        private static float actcaminata(int res5, int res6)
        {
            return (float) 3.3 * res6 * res5;
        }
        
        private static float totalMET(float met1, float met2, float met3)
        {
            return (float) met1 + met2 + met3;
        }

        private static string conductasendentaria(int res7)
        {
            string resultado = "AUSENTE";

            if(res7 > 6)
            {
                resultado = "PRESENTE";
            }

            return resultado;
        }

        private static bool nivelAFAlto(int res1, float MET_AFvigorosa, float MET_AFmoderada, float MET_AFcaminata)
        {
            bool criterio1 = false;
            bool criterio2 = false;

            if (res1 >= 3 && (int) MET_AFvigorosa >= 1500)
            {
                criterio1 = true;
            }

            if ((int) (MET_AFcaminata + MET_AFmoderada) >= 3000)
            {
                criterio2 = true;
            }

            if ((int)(MET_AFcaminata + MET_AFvigorosa) >= 3000)
            {
                criterio2 = true;
            }

            return criterio1 || criterio2;
        }

        private static bool nivelAFModerado(RecuperarRespuestasDto res, float MET_AFvigorosa, float MET_AFmoderada, float MET_AFcaminata)
        {
            bool criterio1 = false;
            bool criterio2 = false;
            bool criterio3 = false;

            if (res.pregunta1 >= 3 && res.pregunta2 >= 20)
            {
                criterio1 = true;
            }

            if ((res.pregunta3 >= 5 && res.pregunta4 >= 30) || (res.pregunta5 >= 5 && res.pregunta6 >=30))
            {
                criterio2 = true;
            }

            if( ((res.pregunta1 + res.pregunta5) >= 5 && (int) (MET_AFvigorosa + MET_AFcaminata) >= 600) || ((res.pregunta3 + res.pregunta5) >= 5 && (int) (MET_AFmoderada + MET_AFcaminata) >= 600))
            {
                criterio3 = true;
            }

            return criterio1 || criterio2 || criterio3;

        }
    }
}
