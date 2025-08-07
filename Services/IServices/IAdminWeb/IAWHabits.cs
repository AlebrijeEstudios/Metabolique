using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Habits_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWHabits
    {
        Task<List<AllHabitDrinkPerUserDto>> GetAllHabitsDrinkPerUserAsync(HabitFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllHabitDrugPerUserDto>> GetAllHabitsDrugsPerUserAsync(HabitFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllHabitSleepPerUserDto>> GetAllHabitsSleepPerUserAsync(HabitFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllMFUsHabitsPerUserDto>> GetMFUsHabitsAsync(PatientFilterDto filter, int page, CancellationToken cancellationToken);

        Task<byte[]> ExportAllHabitsDrinkAsync(HabitFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllHabitsDrugsAsync(HabitFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllHabitsSleepAsync(HabitFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllMFUsHabitsAsync(PatientFilterDto? filter, CancellationToken cancellationToken);
    }
}
