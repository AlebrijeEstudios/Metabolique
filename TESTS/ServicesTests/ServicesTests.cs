using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.Exceptions.Account_Profile.ValidationTimeoutException;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.ValidationValues;
using Microsoft.EntityFrameworkCore;
using AppVidaSana.KeyToken;
using AppVidaSana.Models.Dtos.Reset_Password_Dtos;
using AppVidaSana.Tokens;
using System.Security.Claims;
using System.Security.Cryptography;

namespace AppVidaSana.TESTS.ServicesTests
{
    public class ServicesTests : IServicesTests
    {
        private readonly AppDbContext _bd;
        private readonly IDbContextFactory<AppDbContext> _contextFactory;

        public ServicesTests(AppDbContext bd, IDbContextFactory<AppDbContext> contextFactory)
        {
            _contextFactory = contextFactory;
            _bd = bd;
        }

        public async Task<Guid> CreateAccountAsync(AccountDto values, CancellationToken cancellationToken)
        {
            List<string?> errors = new List<string?>();

            try
            {
                string verifyStatusEmail = await verifyEmailAsync(values.email, cancellationToken);

                if (verifyStatusEmail != "") { errors.Add(verifyStatusEmail); }
            }
            catch (EmailValidationTimeoutException ex)
            {
                errors.Add(ex.Message);
            }

            try
            {
                string verifyStatusPassword = verifyPassword(values.password);

                if (verifyStatusPassword != "") { errors.Add(verifyStatusPassword); }
            }
            catch (PasswordValidationTimeoutException ex)
            {
                errors.Add(ex.Message);
            }

            if (errors.Count > 0) { throw new ValuesInvalidException(errors); }

            Account account = new Account
            {
                username = values.username,
                email = values.email,
                password = BCrypt.Net.BCrypt.HashPassword(values.password)
            };

            ValidationValuesDB.ValidationValues(account);

            PacientDoctor pd = new PacientDoctor
            {
                accountID = account.accountID,
                doctorID = Guid.Parse("f7f8fb3a-8018-4f35-a6b4-181947d447b7")
            };

            _bd.Accounts.Add(account);
            _bd.PacientDoctor.Add(pd);

            if (!Save()) { throw new UnstoredValuesException(); }

            Guid accountID = account.accountID;

            return accountID;
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

        public async Task<TokensDto> RefreshTokenAsync(TokensDto values, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext();

            var account = await context.Accounts.FirstOrDefaultAsync(e => e.accountID == values.accountID, cancellationToken);

            var historial = await context.HistorialRefreshTokens.FirstOrDefaultAsync(e => e.refreshToken == values.refreshToken,
                                                                                        cancellationToken);

            if (account is null || historial is null) { throw new UnstoredValuesException(); }

            var accessToken = CreateAccessToken(account);
            var refreshToken = UpdateRefreshTokenAsync(context, historial, cancellationToken);

            TokensDto response = new TokensDto();

            response.accountID = account.accountID;
            response.accessToken = accessToken;
            response.refreshToken = await refreshToken;

            return response;
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

        private static string CreateAccessToken(Account account)
        {
            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, account.username.ToString()),
                new Claim(ClaimTypes.Email, account.email.ToString())
            };

            DateTime durationToken = DateTime.UtcNow.AddMinutes(1);

            var accessToken = GeneratorTokens.Tokens(KeyTokenEnv.GetKeyTokenEnv(), claims, durationToken);

            return accessToken;
        }

        private async Task<string> CreateRefreshTokenAsync(Guid accountID, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext();

            var refreshToken = GenerateRefreshToken();

            var historial = await context.HistorialRefreshTokens.FirstOrDefaultAsync(e => e.accountID == accountID, cancellationToken);

            if (historial is null)
            {
                HistorialRefreshToken historialRefreshToken = new HistorialRefreshToken
                {
                    accountID = accountID,
                    refreshToken = refreshToken,
                    dateExpiration = DateTime.Now.AddMinutes(3)
                };

                await context.HistorialRefreshTokens.AddAsync(historialRefreshToken, cancellationToken);

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

            historial.refreshToken = refreshToken;

            historial.dateExpiration = DateTime.Now.AddMinutes(3);

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

        private static async Task<string> UpdateRefreshTokenAsync(AppDbContext context, HistorialRefreshToken historial, CancellationToken cancellationToken)
        {
            var refreshToken = GenerateRefreshToken();

            if (historial.dateExpiration < DateTime.Now)
            {
                throw new RefreshTokenExpirationException();
            }

            historial.refreshToken = refreshToken;

            historial.dateExpiration = DateTime.Now.AddMinutes(3);

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

        private async Task<string> verifyEmailAsync(string email, CancellationToken cancellationToken)
        {
            var existingEmail = await _bd.Accounts.AnyAsync(c => c.email == email, cancellationToken);

            if (!RegexPatterns.RegexPatterns.Emailregex.IsMatch(email))
            {
                return "El correo electr&oacute;nico no tiene un formato v&aacute;lido.";
            }

            if (existingEmail!)
            {
                return "Este correo electr&oacute;nico est&aacute; ligado a una cuenta existente.";
            }

            return "";
        }

        private static string verifyPassword(string password)
        {
            if (password.Length < 8)
            {
                return "La contrase&ntilde;a debe tener al menos 8 caracteres.";
            }

            if (!RegexPatterns.RegexPatterns.Passwordregex.IsMatch(password))
            {
                return "La contrase&ntilde;a debe contener al menos un n&uacute;mero, una letra min&uacute;scula o letra may&uacute;scula y un car&aacute;cter alfanum&eacute;rico.";
            }

            return "";
        }

        public async Task<string> DeleteAccountAsync(Guid accountID, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<InfoAccountDto> GetAccountAsync(Guid accountID, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<ProfileDto> UpdateAccountAsync(InfoAccountDto values, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
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

    }
}
