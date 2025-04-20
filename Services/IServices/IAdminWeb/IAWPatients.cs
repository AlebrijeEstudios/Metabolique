using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWPatients
    {
        Task<List<AllPatientsDto>> GetPatientsAsync(PatientFilterDto filter, int page, CancellationToken cancellationToken);

        Task<byte[]> ExportPatientsAsync(PatientFilterDto? filter, CancellationToken cancellationToken);
    }
}
