using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.KeyToken;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.Reset_Password_Dtos;
using AppVidaSana.Services.IServices;
using AppVidaSana.Tokens;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AppVidaSana.Services
{
    public class AuthenticationAuthorizationService : IAuthenticationAuthorization
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public AuthenticationAuthorizationService(IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
        }

        public async Task<TokensDto> LoginAccountAsync(LoginDto login, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext();

            var account = await context.Accounts.FirstOrDefaultAsync(u => u.email == login.email, cancellationToken);

            if (account is null || !BCrypt.Net.BCrypt.Verify(login.password, account.password))
            {
                throw new FailLoginException();
            }

            var accessToken = CreateAccessToken(account);
            var refreshToken = CreateRefreshTokenAsync(account.accountID, cancellationToken);

            TokensDto response = new TokensDto();

            response.accountID = account.accountID;
            response.accessToken = accessToken;
            response.refreshToken = await refreshToken;

            return response;
        }

        public async Task<string> LogoutAccountAsync(Guid accountID, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext();

            var refreshToken = await context.HistorialRefreshTokens.FirstOrDefaultAsync(e => e.accountID == accountID, cancellationToken);

            if (refreshToken is null) { return "Cierre de sesi&oacute;n reciente."; }

            context.HistorialRefreshTokens.Remove(refreshToken!);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {

                throw new UnstoredValuesException();

            }

            return "Cierre de sesi&oacute;n exitoso.";
        }

        public async Task<TokensDto> RefreshTokenAsync(TokensDto values, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext();

            var historial = await context.HistorialRefreshTokens
                .FirstOrDefaultAsync(e => e.refreshToken == values.refreshToken &&
                                        e.accountID == values.accountID, cancellationToken);

            if(historial is null)
            {
                var tokens = context.HistorialRefreshTokens
                .Where(t => t.accountID == values.accountID);

                context.HistorialRefreshTokens.RemoveRange(tokens);

                try
                {
                    await context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception)
                {

                    throw new UnstoredValuesException();

                }

                throw new RefreshTokenExpirationException();
            }

            if (historial.dateExpiration < DateTime.UtcNow)
            {
                context.HistorialRefreshTokens.Remove(historial);

                try
                {
                    await context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception)
                {

                    throw new UnstoredValuesException();

                }

                throw new RefreshTokenExpirationException();
            }

            var account = await context.Accounts.FirstOrDefaultAsync(e => e.accountID == historial.accountID, cancellationToken);

            if (account is null) { throw new RefreshTokenExpirationException(); }

            var accessToken = CreateAccessToken(account);
            var refreshToken = UpdateRefreshTokenAsync(context, historial, cancellationToken);

            TokensDto response = new TokensDto();

            response.accountID = account.accountID;
            response.accessToken = accessToken;
            response.refreshToken = await refreshToken;

            return response;
        }

        private static string CreateAccessToken(Account account)
        {
            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, account.username.ToString()),
                new Claim(ClaimTypes.Email, account.email.ToString()),
                new Claim("typ", "access")
            };

            DateTime durationToken = DateTime.UtcNow.AddHours(1);

            var accessToken = GeneratorTokens.Tokens(KeyTokenEnv.GetKeyTokenEnv(), claims, durationToken);

            return accessToken;
        }

        private async Task<string> CreateRefreshTokenAsync(Guid accountID, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext(); 
   
            var historial = await context.HistorialRefreshTokens.FirstOrDefaultAsync(e => e.accountID == accountID, cancellationToken);

            if (historial is null)
            {
                var refreshToken = GenerateRefreshToken();

                HistorialRefreshToken historialRefreshToken = new HistorialRefreshToken
                {
                    accountID = accountID,
                    refreshToken = refreshToken,
                    dateExpiration = DateTime.UtcNow.AddDays(14)
                };

                await context.HistorialRefreshTokens.AddAsync(historialRefreshToken, cancellationToken);

                try {
                    await context.SaveChangesAsync(cancellationToken);
                }
                catch (Exception)
                {

                    throw new UnstoredValuesException();

                }

                return refreshToken;
            }

            var refreshTokenUpdate = await UpdateRefreshTokenAsync(context, historial, cancellationToken);

            return refreshTokenUpdate;
        }    

        private static async Task<string> UpdateRefreshTokenAsync(AppDbContext context, HistorialRefreshToken historial, CancellationToken cancellationToken)
        {
            var refreshToken = GenerateRefreshToken();

            historial.refreshToken = refreshToken;

            historial.dateExpiration = DateTime.UtcNow.AddDays(14);

            try
            {
                await context.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {

                throw new UnstoredValuesException();

            }

            return refreshToken;
        }

        private static string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
