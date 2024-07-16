using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;

namespace AppVidaSana.Services.IServices.ISeguimientos_Mensuales
{
    public interface IMFUsExercise
    {
        string SaveAnswers(SaveResponsesDto res);

        RetrieveResponsesDto RetrieveAnswers(Guid id, string month, int year);

        bool Save();
    }
}
