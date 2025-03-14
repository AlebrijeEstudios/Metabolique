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
    }
}
