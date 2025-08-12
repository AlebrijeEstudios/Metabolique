using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos;
using AppVidaSana.Models.Dtos.Medication_Dtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWMedication
    {
        Task<List<InfoMedicationDto>> GetAllInfoMedicationsPerUserAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllSideEffectsPerUserDto>> GetAllSideEffectsAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllMFUsMedicationsPerUserDto>> GetMFUsMedicationsAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);

        Task<byte[]> ExportAllPeriodMedicationsAsync(FilterAdminDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllSideEffectsAsync(FilterAdminDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllMFUsMedicationAsync(FilterAdminDto? filter, CancellationToken cancellationToken);
    }
}
