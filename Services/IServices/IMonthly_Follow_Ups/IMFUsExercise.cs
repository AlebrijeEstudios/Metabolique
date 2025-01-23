using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Exercise_Dtos;

namespace AppVidaSana.Services.IServices.IMonthly_Follow_Ups
{
    public interface IMFUsExercise
    {
        Task<RetrieveResponsesExerciseDto?> RetrieveAnswersAsync(Guid accountID, int month, int year, CancellationToken cancellationToken);

        Task<RetrieveResponsesExerciseDto?> SaveAnswersAsync(SaveResponsesExerciseDto values, CancellationToken cancellationToken);

        Task<RetrieveResponsesExerciseDto?> UpdateAnswersAsync(UpdateResponsesExerciseDto values, CancellationToken cancellationToken);

        bool Save();
    }
}
