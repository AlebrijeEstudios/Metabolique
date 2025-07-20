using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos;
using AppVidaSana.Models.Dtos.Medication_Dtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWMedication
    {
        Task<List<InfoMedicationDto>> GetAllInfoMedicationsPerUserAsync(PeriodMedicationsFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllSideEffectsPerUserDto>> GetAllSideEffectsAsync(SideEffectsFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllMFUsMedicationsPerUserDto>> GetMFUsMedicationsAsync(MFUsMedicationFilterDto filter, int page, CancellationToken cancellationToken);

        Task<byte[]> ExportAllPeriodMedicationsAsync(PeriodMedicationsFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllSideEffectsAsync(SideEffectsFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllMFUsMedicationAsync(MFUsMedicationFilterDto? filter, CancellationToken cancellationToken);
    }
}
