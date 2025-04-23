using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWPatientsService :IAWPatients
    {
        private readonly AppDbContext _bd;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AWPatientsService(AppDbContext bd, IHttpContextAccessor httpContextAccessor)
        {
            _bd = bd;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<AllPatientsDto>> GetPatientsAsync(PatientFilterDto filter, int page, CancellationToken cancellationToken)
        {
            var role = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (role is null) { throw new UnstoredValuesException(); }

            if (role == "Admin")
            {
                var profiles = await GetQueryPatientsAsync(filter, page, false, 0, cancellationToken);

                var accountProfileDTOs = profiles.Select(profile => new AllPatientsDto
                {
                    accountID = profile.accountID,
                    uiemID = profile.uiemID,
                    username = profile.account?.username ?? "N/A",
                    email = profile.account?.email ?? "N/A",
                    birthDate = profile.birthDate,
                    sex = profile.sex,
                    stature = profile.stature,
                    weight = profile.weight,
                    protocolToFollow = profile.protocolToFollow

                }).ToList();

                return accountProfileDTOs;
            }

            return [];
        }

        public async Task<byte[]> ExportPatientsAsync(PatientFilterDto? filter, CancellationToken cancellationToken) 
        {
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("AccountID,UiemID,UserName,Email,BirthDate,Sex,Stature,Weight,ProtocolToFollow");

                while (currentPage >= 0)
                {
                    var profiles = await GetQueryPatientsAsync(filter, 0, true, currentPage, cancellationToken);

                    if (profiles.Count == 0)
                    {
                        currentPage = -1;
                    }
                    else 
                    { 
                        foreach (var p in profiles)
                        {
                            var csvLine = $"{p.accountID},{p.uiemID ?? "N/A"},{p.account!.username},{p.account!.email},{p.birthDate},{p.sex},{p.stature},{p.weight},{p.protocolToFollow}";

                            await streamWriter.WriteLineAsync(csvLine);
                        }

                        currentPage++;
                    }
                }

                await streamWriter.FlushAsync(cancellationToken);

                return memoryStream.ToArray();
            }
        }

        private async Task<List<Profiles>> GetQueryPatientsAsync(PatientFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken)
        {
            List<Profiles> patients = new List<Profiles>();

            var query = _bd.Profiles
                           .Include(f => f.account)
                           .AsQueryable();

            if (filter != null)
            {
                query = query.Where(p => _bd.PacientDoctor
                                        .Where(pd => pd.doctorID == filter.doctorID)
                                        .Select(pd => pd.accountID)
                                        .Contains(p.account!.accountID));

                if (!string.IsNullOrWhiteSpace(filter.accountID.ToString()))
                    query = query.Where(f => f.account!.accountID.ToString().Contains(filter.accountID.ToString() ?? ""));

                if (!string.IsNullOrWhiteSpace(filter.username))
                    query = query.Where(f => f.account!.username.Contains(filter.username ?? ""));

                if (!string.IsNullOrWhiteSpace(filter.uiemID))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.account!.accountID && p.uiemID == filter.uiemID));

                if (!string.IsNullOrWhiteSpace(filter.month.ToString()))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.account!.accountID && p.birthDate.Month == filter.month));

                if (!string.IsNullOrWhiteSpace(filter.year.ToString()))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.account!.accountID && p.birthDate.Year == filter.year));

                if (!string.IsNullOrWhiteSpace(filter.sex))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.account!.accountID && p.sex == filter.sex));

                if (!string.IsNullOrWhiteSpace(filter.protocolToFollow))
                    query = query.Where(f => _bd.Profiles
                                    .Any(p => p.accountID == f.account!.accountID && p.protocolToFollow == filter.protocolToFollow));

            }

            if (!export)
            {
                patients = await query
                            .Skip((page - 1) * 10)
                            .Take(10)
                            .ToListAsync(cancellationToken);
            }
            else { 
                patients = await query
                            .Skip(currentPage * 1000)
                            .Take(1000)
                            .ToListAsync(cancellationToken);
            }

            return patients;
        }
    }
}
