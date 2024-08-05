using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Exercise_Dtos;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;

namespace AppVidaSana.Services.IServices.ISeguimientos_Mensuales
{
    public interface IMFUsExercise
    {
        SaveResultsExerciseDto SaveAnswers(SaveResponsesExerciseDto res);

        RetrieveResponsesExerciseDto RetrieveAnswers(Guid id, string month, int year);

        public string SaveResults(SaveResultsExerciseDto res);

        bool Save();
    }
}
