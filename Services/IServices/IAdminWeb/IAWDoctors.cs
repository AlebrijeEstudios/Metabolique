using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Doctor_AWDtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWDoctors
    {
        Task<List<AllDoctorsDto>> GetDoctorsAsync(DoctorFilterDto filter, int page, CancellationToken cancellationToken);

        Task<AllDoctorsDto> InsertDoctorAsync(DoctorDto values, CancellationToken cancellationToken);

        Task<AllDoctorsDto> UpdateDoctorAsync(AllDoctorsDto values, CancellationToken cancellationToken);

        Task<string> DeleteDoctorAsync(Guid doctorID, CancellationToken cancellationToken);
    }
}
