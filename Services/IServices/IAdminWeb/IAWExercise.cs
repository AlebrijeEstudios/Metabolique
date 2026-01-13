using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Exercise_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWExercise
    {
        Task<List<AllExercisesPerUserDto>> GetAllExercisesPerUserAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllMFUsExercisePerUserDto>> GetMFUsExerciseAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);

        Task<byte[]> ExportAllExercisesAsync(FilterAdminDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllMFUsExerciseAsync(FilterAdminDto? filter, CancellationToken cancellationToken);
        
        Task<List<AllActiveMinutesPerExerciseDto>> GetAllActiveMinutesPerExerciseAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);
        
        Task<byte[]> ExportAllActivesMinutesAsync(FilterAdminDto? filter, CancellationToken cancellationToken);
    }
}
