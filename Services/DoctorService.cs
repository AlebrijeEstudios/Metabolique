using AppVidaSana.Data;
using AppVidaSana.Models.Dtos.Doctor_Dtos;
using AppVidaSana.Services.IServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services
{
    public class DoctorService : IDoctor
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public DoctorService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
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
