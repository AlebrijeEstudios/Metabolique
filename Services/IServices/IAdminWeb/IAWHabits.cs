namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWHabits
    {
        Task<byte[]> ExportAllHabitsDrinkAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllHabitsDrugsAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllHabitsSleepAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllMFUsHabitsAsync(CancellationToken cancellationToken);
    }
}
