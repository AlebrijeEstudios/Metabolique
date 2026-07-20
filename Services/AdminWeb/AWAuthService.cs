using AppVidaSana.Data;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.KeyToken;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.Services.IServices.IAdminWeb;
using AppVidaSana.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWAuthService : IAWAuth
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public AWAuthService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<TokenAdminDto> LoginAdminAsync(LoginAdminDto login, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext();

            var account = await context.Doctors.FirstOrDefaultAsync(u => u.email == login.username, cancellationToken);

            if (account is null || !BCrypt.Net.BCrypt.Verify(login.password, account.password))
            {
                throw new FailLoginException();
            }

            var role = await context.Roles.FirstOrDefaultAsync(e => e.roleID == account.roleID, cancellationToken);

            var accessToken = CreateAccessTokenAdminAsync(account, role!.role);

            TokenAdminDto response = new TokenAdminDto();

            response.doctorID = account.doctorID;
            response.accessToken = accessToken;
            response.role = role!.role;

            return response;
        }

        private static string CreateAccessTokenAdminAsync(Doctors account, string role)
        {
            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, account.username.ToString()),
                new Claim(ClaimTypes.Email, account.email.ToString()),
                new Claim(ClaimTypes.Role, role),
                new Claim("doctorID", account.doctorID.ToString()),
                new Claim("typ", "access")
            };

            DateTime durationToken = DateTime.UtcNow.AddHours(2);

            var accessToken = GeneratorTokens.Tokens(KeyTokenEnv.GetKeyTokenEnv(), claims, durationToken);

            return accessToken;
        }
    }
}
