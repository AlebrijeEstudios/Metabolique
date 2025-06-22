using AppVidaSana.Data;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWPatientsService :IAWPatients
    {
        private readonly AppDbContext _bd;
        //private readonly IHttpContextAccessor _httpContextAccessor;

        public AWPatientsService(AppDbContext bd)
        {
            _bd = bd;
        }

        public async Task<List<AllPatientsDto>> GetPatientsAsync(PatientFilterDto filter, int page, CancellationToken cancellationToken)
        {
            /*var role = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

            if (role is null) { throw new UnstoredValuesException(); }*/

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
                protocolToFollow = profile?.protocol?.protocolToFollow

            }).ToList();

            return accountProfileDTOs;
        }

        public async Task<byte[]> ExportPatientsAsync(PatientFilterDto? filter, CancellationToken cancellationToken) 
        {
            int currentPage = 0;
            List<Profiles> profiles;

            using var memoryStream = new MemoryStream();
            using var streamWriter = new StreamWriter(memoryStream);

            await streamWriter.WriteLineAsync("AccountID,UiemID,UserName,Email,BirthDate,Sex,Stature,Weight,ProtocolToFollow");

            do
            {
                profiles = await GetQueryPatientsAsync(filter, 0, true, currentPage, cancellationToken);

                foreach (var p in profiles)
                {
                    var csvLine = $"{p.accountID},{p.uiemID ?? "N/A"},{p.account!.username},{p.account!.email},{p.birthDate},{p.sex},{p.stature},{p.weight},{p.protocol!.protocolToFollow}";

                    await streamWriter.WriteLineAsync(csvLine);
                }

                currentPage++;

            } while (profiles.Count > 0);

            await streamWriter.FlushAsync(cancellationToken);

            return memoryStream.ToArray();
        }

        private async Task<List<Profiles>> GetQueryPatientsAsync(PatientFilterDto? filter, int page, bool export, int currentPage, CancellationToken cancellationToken)
        {
            List<Profiles> patients;

            var query = _bd.Profiles
                           .Include(f => f.account)
                           .Include(f => f.protocol)
                           .AsQueryable();

            if (filter != null)
            {
                query = FilterPatientsByPatient(query, filter);
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

        private IQueryable<Profiles> FilterPatientsByPatient(IQueryable<Profiles> query, PatientFilterDto filter) 
        {
            if (!string.IsNullOrWhiteSpace(filter.doctorID.ToString()))
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
                                .Any(p => p.accountID == f.account!.accountID && p.protocol!.protocolToFollow == filter.protocolToFollow));

            return query;
        }
    }
}
