using AppVidaSana.Models.Dtos.Account_Profile_Dtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWPatients
    {
        Task<List<InfoAccountDto>> GetPatientsAsync(Guid doctorID, int page, CancellationToken cancellationToken);

        Task<byte[]> ExportAllPatientsAsync(CancellationToken cancellationToken);
    }
}
