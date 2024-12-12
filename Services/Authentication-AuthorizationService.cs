using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.KeyToken;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.Reset_Password_Dtos;
using AppVidaSana.Services.IServices;
using AppVidaSana.Tokens;
using AppVidaSana.ValidationValues;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AppVidaSana.Services
{
    public class AuthenticationAuthorizationService : IAuthenticationAuthorization
    {
        private readonly AppDbContext _bd;
        private readonly ValidationValuesDB _validationValues;
        private readonly GeneratorTokens _generatorTokens;
        private readonly KeyTokenEnv _keyToken;

        public AuthenticationAuthorizationService(AppDbContext bd)
        {
            _bd = bd;
            _validationValues = new ValidationValuesDB();
            _generatorTokens = new GeneratorTokens();
            _keyToken = new KeyTokenEnv();
        }

        public async Task<TokensDto> LoginAccountAsync(LoginDto login, CancellationToken cancellationToken)
        {
            var account = await _bd.Accounts.FirstOrDefaultAsync(u =>
                                                                 u.email == login.email, cancellationToken);

            if (account is null || !BCrypt.Net.BCrypt.Verify(login.password, account.password))
            {
                throw new FailLoginException();
            }

            var accessToken = await CreateTokenAsync(account, cancellationToken);
            var refreshToken = await CreateRefreshTokenAsync(account.accountID, cancellationToken);

            TokensDto response = new TokensDto()
            {
                accountID = account.accountID,
                accessToken = accessToken,
                refreshToken = refreshToken
            };

            return response;
        }

        public async Task<TokensDto> RefreshTokenAsync(TokensDto values, CancellationToken cancellationToken)
        {
            var principal = _generatorTokens.GetPrincipalFromExpiredToken(values.accessToken, _keyToken.GetKeyTokenEnv());

            var user = await _bd.Accounts.FirstOrDefaultAsync(e => e.accountID == values.accountID, cancellationToken);

            var historial = await _bd.HistorialRefreshTokens.FirstOrDefaultAsync(e => e.refreshToken == values.refreshToken, cancellationToken);

            if(user is null || historial is null) { throw new UnstoredValuesException(); }

            var accessToken = await CreateTokenAsync(user, cancellationToken);
            var refreshToken = await CreateRefreshTokenAsync(user.accountID, cancellationToken);

            TokensDto response = new TokensDto()
            {
                accountID = user.accountID,
                accessToken = accessToken,
                refreshToken = refreshToken
            };

            return response;
        }

        private async Task<string> CreateTokenAsync(Account account, CancellationToken cancellationToken)
        {
            var role = await _bd.Roles.FirstOrDefaultAsync(e => e.roleID == account.roleID, cancellationToken);

            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, account.username.ToString()),
                new Claim(ClaimTypes.Email, account.email.ToString()),
                new Claim(ClaimTypes.Role, role!.role)
            };

            DateTime durationToken = DateTime.UtcNow.AddMinutes(30);

            var accessToken = _generatorTokens.Tokens(_keyToken.GetKeyTokenEnv(), claims, durationToken);

            return accessToken;
        }

        private async Task<string> CreateRefreshTokenAsync(Guid accountID, CancellationToken cancellationToken)
        {
            var refreshToken = GenerateRefreshToken();

            var historial = await _bd.HistorialRefreshTokens.FirstOrDefaultAsync(e => e.accountID == accountID, cancellationToken);
            
            if(historial is null)
            {
                HistorialRefreshToken historialRefreshToken = new HistorialRefreshToken
                {
                    accountID = accountID,
                    refreshToken = refreshToken,
                    dateExpiration = DateTime.Now.AddDays(7)
                };

                _validationValues.ValidationValues(historialRefreshToken);

                _bd.HistorialRefreshTokens.Add(historialRefreshToken);

                if (!Save()) { throw new UnstoredValuesException(); }

                return refreshToken;
            }

            if (historial.dateExpiration <= DateTime.Now)
            {
                historial.refreshToken = refreshToken;

                historial.dateExpiration = DateTime.Now.AddDays(7);

                UpdateRefreshToken(historial);

                return refreshToken;
            }

            historial.refreshToken = refreshToken;

            UpdateRefreshToken(historial);

            return refreshToken;
        }

        private void UpdateRefreshToken(HistorialRefreshToken values)
        {
            _validationValues.ValidationValues(values);

            _bd.HistorialRefreshTokens.Update(values);

            if (!Save()) { throw new UnstoredValuesException(); }
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

        public bool Save()
        {
            try
            {
                return _bd.SaveChanges() >= 0;
            }
            catch (Exception)
            {
                return false;

            }
        }
    }
}
