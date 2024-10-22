using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.Reset_Password_Dtos;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AppVidaSana.Services
{
    public class Authentication_AuthorizationService : IAuthentication_Authorization
    {
        private readonly AppDbContext _bd;
        private readonly string keyToken;
        private ValidationValuesDB _validationValues;

        public Authentication_AuthorizationService(AppDbContext bd)
        {
            _bd = bd;
            _validationValues = new ValidationValuesDB();
            keyToken = Environment.GetEnvironmentVariable("TOKEN") ?? Environment.GetEnvironmentVariable("TOKEN_Replacement");
        }

        public async Task<TokensDto> LoginAccount(LoginDto login, CancellationToken cancellationToken)
        {
            var account = await _bd.Accounts.FirstOrDefaultAsync(u =>
                                          u.email.ToLower() == login.email.ToLower(), cancellationToken);

            if (account == null || !BCrypt.Net.BCrypt.Verify(login.password, account.password))
            {
                throw new FailLoginException();
            }

            var accessToken = await CreateToken(account);
            var refreshToken = await CreateRefreshToken(account.accountID);

            TokensDto response = new TokensDto()
            {
                accountID = account.accountID,
                accessToken = accessToken,
                refreshToken = refreshToken
            };

            return response;
        }

        public async Task<TokensDto> RefreshToken(TokensDto values, CancellationToken cancellationToken)
        {
            var principal = GetPrincipalFromExpiredToken(values.accessToken);

            var user = await _bd.Accounts.FirstOrDefaultAsync(e => e.accountID == values.accountID
                                                              && e.username == principal.Identity.Name, cancellationToken);

            var historial = await _bd.HistorialRefreshTokens.FirstOrDefaultAsync(e => e.refreshToken == values.refreshToken);

            if(user is null || historial is null)
            {
                throw new UnstoredValuesException();
            }

            var accessToken = await CreateToken(user);
            var refreshToken = await CreateRefreshToken(user.accountID);

            TokensDto response = new TokensDto()
            {
                accountID = user.accountID,
                accessToken = accessToken,
                refreshToken = refreshToken
            };

            return response;
        }

        private async Task<string> CreateToken(Account account)
        {
            var role = await _bd.Roles.FirstOrDefaultAsync(e => e.roleID == account.roleID);

            var tokenHandler = new JwtSecurityTokenHandler();
            var keyBytes = Encoding.ASCII.GetBytes(keyToken);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, account.username.ToString()),
                        new Claim(ClaimTypes.Email, account.email.ToString()),
                        new Claim(ClaimTypes.Role, role.role)
                }),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new(new SymmetricSecurityKey(keyBytes), SecurityAlgorithms.HmacSha256Signature)
            };

            var createToken = tokenHandler.CreateToken(tokenDescriptor);

            var accessToken = tokenHandler.WriteToken(createToken);

            return accessToken;
        }

        private async Task<string> CreateRefreshToken(Guid accountID)
        {
            var refreshToken = GenerateRefreshToken();

            var historial = await _bd.HistorialRefreshTokens.FirstOrDefaultAsync(e => e.accountID == accountID);
            
            if (historial != null && !(historial.dateExpiration <= DateTime.Now))
            {
                historial.refreshToken = refreshToken;

                UpdateRefreshToken(historial);

                return refreshToken;
            }

            if (historial.dateExpiration <= DateTime.Now)
            {
                historial.refreshToken = refreshToken;

                historial.dateExpiration = DateTime.Now.AddDays(7);

                UpdateRefreshToken(historial);

                return refreshToken;
            }

            HistorialRefreshToken historialRefreshToken = new HistorialRefreshToken
            {
                accountID = accountID,
                refreshToken = refreshToken,
                dateExpiration = DateTime.Now.AddDays(7)
            };

            _validationValues.ValidationValues(historialRefreshToken);

            await _bd.HistorialRefreshTokens.AddAsync(historialRefreshToken);

            if (!Save()) { throw new UnstoredValuesException(); }

            return refreshToken;
        }

        private void UpdateRefreshToken(HistorialRefreshToken values)
        {
            _validationValues.ValidationValues(values);

            _bd.HistorialRefreshTokens.Update(values);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        private string GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes(keyToken);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg
                .Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
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
