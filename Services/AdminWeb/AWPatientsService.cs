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
                var accounts = await _bd.Accounts
                                .Skip((page - 1) * 10)
                                .Take(10)
                                .ToListAsync(cancellationToken);

                var profiles = await _bd.Profiles
                                .Skip((page - 1) * 10)
                                .Take(10)
                                .ToListAsync(cancellationToken);

                var accountProfileDTOs = accounts.Select(account => new InfoAccountDto
                {
                    accountID = account.accountID,
                    username = account.username,
                    email = account.email,
                    birthDate = profiles.FirstOrDefault(profile => profile.accountID == account.accountID).birthDate,
                    sex = profiles.FirstOrDefault(profile => profile.accountID == account.accountID).sex,
                    stature = profiles.FirstOrDefault(profile => profile.accountID == account.accountID).stature,
                    weight = profiles.FirstOrDefault(profile => profile.accountID == account.accountID).weight,
                    protocolToFollow = profiles.FirstOrDefault(profile => profile.accountID == account.accountID).protocolToFollow
                }).ToList();

                return accountProfileDTOs;
            }

            return [];
        }
    }
}
