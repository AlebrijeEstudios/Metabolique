using AppVidaSana.Data;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;
using AppVidaSana.Services.IServices.ISeguimientos_Mensuales;
using AutoMapper;

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
            if (res == null)
            {
                return "No se guardaron los datos, intentelo de nuevo";
            }


            return "No se guardaron los datos, intentelo de nuevo";

        }

        public RecuperarRespuestasDto RecuperarRespuestas(Guid id, string mes, int año)
        {
            throw new NotImplementedException();
        }


       /* private float actvigorosa(int res1, int res2)
        {

        }

        private float actmoderada(int res5, int res6)
        {

        }

        private float actcaminata(int res5, int res6)
        {

        }

        private float totalMET(int res1, int res2, int res3)
        {

        }

        private string conductasendentaria(int res7)
        {

        }

        private bool nivelAFAlto(int res1, int res3, int res5, float MET_AFvigorosa, float totalMET)
        {

        }

        private bool nivelAFModerado(RecuperarRespuestasDto res, float MET_AFvigorosa, float MET_AFmoderada, float MET_AFcaminata)
        {

        }*/


    }
}
