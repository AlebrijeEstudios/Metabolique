using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Exercise_Dtos;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;

namespace AppVidaSana.Services.IServices.ISeguimientos_Mensuales
{
    public interface IMFUsExercise
    {
        Task<RetrieveResponsesExerciseDto?> RetrieveAnswersAsync(Guid accountID, int month, int year, CancellationToken cancellationToken);
        
        Task<RetrieveResponsesExerciseDto?> SaveAnswersAsync(SaveResponsesExerciseDto values, CancellationToken cancellationToken);

        Task<RetrieveResponsesExerciseDto?> UpdateAnswersAsync(UpdateResponsesExerciseDto values, CancellationToken cancellationToken);

        bool Save();
    }
}
