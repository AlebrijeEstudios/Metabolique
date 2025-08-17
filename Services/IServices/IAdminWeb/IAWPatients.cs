using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWPatients
    {
        Task<List<AllPatientsDto>> GetPatientsAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);

        Task<byte[]> ExportPatientsAsync(FilterAdminDto? filter, CancellationToken cancellationToken);
    }
}
