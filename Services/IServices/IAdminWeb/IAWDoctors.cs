using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Doctor_AWDtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWDoctors
    {
        Task<string> InsertDoctorAsync(DoctorDto values, CancellationToken cancellationToken);
    }
}
