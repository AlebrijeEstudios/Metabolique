using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Exercise_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWExercise
    {
        Task<List<AllExercisesPerUserDto>> GetAllExercisesPerUserAsync(ExerciseFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllActiveMinutesPerExerciseDto>> GetAllActiveMinutesPerExerciseAsync(ActiveMinutesFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllMFUsExercisePerUserDto>> GetMFUsExerciseAsync(PatientFilterDto filter, int page, CancellationToken cancellationToken);

        Task<byte[]> ExportAllExercisesAsync(ExerciseFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllActivesMinutesAsync(ActiveMinutesFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllMFUsExerciseAsync(PatientFilterDto? filter, CancellationToken cancellationToken);
    }
}
