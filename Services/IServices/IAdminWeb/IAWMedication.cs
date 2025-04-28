using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWMedication
    {
        Task<List<AllPeriodsMedicationsPerUserDto>> GetAllPeriodMedicationsPerUserAsync(PeriodMedicationsFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllSideEffectsPerUserDto>> GetAllSideEffectsAsync(SideEffectsFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllMFUsMedicationsPerUserDto>> GetMFUsMedicationsAsync(MFUsMedicationFilterDto filter, int page, CancellationToken cancellationToken);

        Task<byte[]> ExportAllPeriodMedicationsAsync(PeriodMedicationsFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllSideEffectsAsync(SideEffectsFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllMFUsMedicationAsync(MFUsMedicationFilterDto? filter, CancellationToken cancellationToken);




        Task<byte[]> ExportAllPeriodsMedicationsAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllDaysConsumedOfMedAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllConsumptionTimesAsync(CancellationToken cancellationToken);
    }
}
