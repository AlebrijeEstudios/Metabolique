using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Habits_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWHabits
    {
        Task<List<AllHabitDrinkPerUserDto>> GetAllHabitsDrinkPerUserAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllHabitDrugPerUserDto>> GetAllHabitsDrugsPerUserAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllHabitSleepPerUserDto>> GetAllHabitsSleepPerUserAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllMFUsHabitsPerUserDto>> GetMFUsHabitsAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);

        Task<byte[]> ExportAllHabitsDrinkAsync(FilterAdminDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllHabitsDrugsAsync(FilterAdminDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllHabitsSleepAsync(FilterAdminDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllMFUsHabitsAsync(FilterAdminDto? filter, CancellationToken cancellationToken);
    }
}
