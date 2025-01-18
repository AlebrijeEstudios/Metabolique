using AppVidaSana.Models.Dtos.Exercise_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IExercise
    {
        Task<List<ExerciseDto>> GetExercisesAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken); 

        Task<InfoGeneralExerciseDto> GetInfoGeneralExercisesAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken);

        Task<ExerciseDto> AddExerciseAsync(AddExerciseDto values, CancellationToken cancellationToken);

        Task<ExerciseDto> UpdateExerciseAsync(ExerciseDto values, CancellationToken cancellationToken);

        Task<string> DeleteExerciseAsync(Guid exerciseID, CancellationToken cancellationToken);

        bool Save();
    }
}
