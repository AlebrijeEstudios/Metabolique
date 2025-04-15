using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWPatientsService :IAWPatients
    {
        private readonly AppDbContext _bd;
        public AWPatientsService(AppDbContext bd)
        {
            _bd = bd;
        }

        public async Task<List<InfoAccountDto>> GetPatientsAsync(Guid doctorID, int page, CancellationToken cancellationToken)
        {
            var infoDoctor = await _bd.Doctors.FindAsync(new object[] { doctorID }, cancellationToken);

            if (infoDoctor is null) { throw new UnstoredValuesException(); }

            var role = await _bd.Roles.FindAsync(new object[] { infoDoctor.roleID }, cancellationToken);

            if (role is null) { throw new UnstoredValuesException(); }

            if (role.role == "Admin")
            {
                var profiles = await _bd.Profiles
                                .Include(f => f.account)
                                .Skip((page - 1) * 10)
                                .Take(10)
                                .ToListAsync(cancellationToken);

                var accountProfileDTOs = profiles.Select(profile => new InfoAccountDto
                {
                    accountID = profile.accountID,
                    username = profile.account?.username ?? "N/A",
                    email = profile.account?.email ?? "N/A",
                    birthDate = profile!.birthDate,
                    sex = profile!.sex,
                    stature = profile!.stature,
                    weight = profile!.weight,
                    protocolToFollow = profile!.protocolToFollow

                }).ToList();

                return accountProfileDTOs;
            }

            return [];
        }

        public async Task<byte[]> ExportAllPatientsAsync(CancellationToken cancellationToken) 
        {
            const int pageSize = 1000;
            int currentPage = 0;

            using (var memoryStream = new MemoryStream())
            using (var streamWriter = new StreamWriter(memoryStream))
            {
                await streamWriter.WriteLineAsync("AccountID,UiemID,UserName,Email,BirthDate,Sex,Stature,Weight,ProtocolToFollow");

                while (currentPage >= 0)
                {
                    var profiles = await _bd.Profiles
                                .Include(f => f.account)
                                .Skip(currentPage * pageSize)
                                .Take(pageSize)
                                .ToListAsync(cancellationToken);

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
    }
}
