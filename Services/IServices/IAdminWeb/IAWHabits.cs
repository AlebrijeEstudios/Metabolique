using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Habits_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWHabits
    {
        Task<List<AllHabitDrinkPerUserDto>> GetAllHabitsDrinkPerUserAsync(HabitDrinkFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllHabitDrugPerUserDto>> GetAllHabitsDrugsPerUserAsync(HabitDrugFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllHabitSleepPerUserDto>> GetAllHabitsSleepPerUserAsync(HabitSleepFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllMFUsHabitsPerUserDto>> GetMFUsHabitsAsync(PatientFilterDto filter, int page, CancellationToken cancellationToken);

        Task<byte[]> ExportAllHabitsDrinkAsync(HabitDrinkFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllHabitsDrugsAsync(HabitDrugFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllHabitsSleepAsync(HabitSleepFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllMFUsHabitsAsync(PatientFilterDto? filter, CancellationToken cancellationToken);
    }
}
