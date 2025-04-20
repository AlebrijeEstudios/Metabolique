using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;

namespace AppVidaSana.Services.IServices
{
    public interface IExportToZip
    {
        Task<byte[]> GenerateAllSectionsZipAsync(CancellationToken cancellationToken);

        Task<byte[]> GenerateOnlyPatientsZipAsync(PatientFilterDto? filter, string typeExport, CancellationToken cancellationToken);
    }
}
