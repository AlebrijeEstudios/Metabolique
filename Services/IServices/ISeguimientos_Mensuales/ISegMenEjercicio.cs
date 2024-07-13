using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;

namespace AppVidaSana.Services.IServices.ISeguimientos_Mensuales
{
    public interface ISegMenEjercicio
    {
        string GuardarRespuestas(GuardarRespuestasDto res);

        RecuperarRespuestasDto RecuperarRespuestas(Guid id, string mes, int año);

        bool Guardar();
    }
}
