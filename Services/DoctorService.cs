using AppVidaSana.Data;
using AppVidaSana.Models.Dtos.Doctor_Dtos;
using AppVidaSana.Services.IServices;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services
{
    public class DoctorService : IDoctor
    {
        private readonly AppDbContext _bd;

        public DoctorService(AppDbContext bd)
        {
            _bd = bd;
        }

        public async Task<List<DoctorDto>> GetDoctorsAsync(CancellationToken cancellationToken)
        {
            return await _bd.Doctors
                    .Select(d => new DoctorDto
                    {
                        doctorID = d.doctorID,
                        username = d.username
                    })
                    .ToListAsync(cancellationToken);
        }
    }
}
