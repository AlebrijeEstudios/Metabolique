using AppVidaSana.Models.Dtos.Doctor_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IDoctor
    {
        Task<List<DoctorDto>> GetDoctorsAsync(CancellationToken cancellationToken);
    }
}
