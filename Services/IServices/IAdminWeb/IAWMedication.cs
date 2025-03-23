namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWMedication
    {
        Task<byte[]> ExportAllPeriodsMedicationsAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllDaysConsumedOfMedAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllConsumptionTimesAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllSideEffectsAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllMFUsMedicationAsync(CancellationToken cancellationToken);
    }
}
