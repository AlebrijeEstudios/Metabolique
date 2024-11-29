using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Account_Profile.ResetPasswordException;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.KeyToken;
using AppVidaSana.Models.Dtos.Reset_Password_Dtos;
using AppVidaSana.Services.IServices;
using AppVidaSana.Tokens;
using AppVidaSana.ValidationValues;
using Azure;
using Azure.Communication.Email;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace AppVidaSana.Services
{
    public class ResetPassswordService : IResetPassword
    {
        private readonly AppDbContext _bd;
        private readonly ValidationValuesDB _validationValues;
        private readonly GeneratorTokens _generatorTokens;
        private readonly KeyTokenEnv _keyToken;

        public ResetPassswordService(AppDbContext bd)
        {
            _bd = bd;
            _validationValues = new ValidationValuesDB();
            _generatorTokens = new GeneratorTokens();
            _keyToken = new KeyTokenEnv();
        }
        
        public async Task<string> PasswordResetTokenAsync(EmailDto value, CancellationToken cancellationToken)
        {
            var account = await _bd.Accounts.FirstOrDefaultAsync(e => e.email == value.email, cancellationToken);

            if (account == null) { throw new EmailNotSendException(); }

            Claim[] claims = new Claim[]
            {
                new Claim(ClaimTypes.Name, account.username.ToString()),
                new Claim(ClaimTypes.Email, account.email.ToString())
            };

            DateTime durationToken = DateTime.UtcNow.AddMinutes(15);

            var accessToken = _generatorTokens.Tokens(_keyToken.GetKeyTokenEnv(), claims, durationToken);

            return accessToken;
        }

        public async void SendEmailAsync(string email, string resetLink)
        {
            List<string?> errors = new List<string?>();

            try
            {
                EmailClient emailClient = new EmailClient(Environment.GetEnvironmentVariable("EMAIL_API"));
                await emailClient.SendAsync(
                    WaitUntil.Completed,
                    senderAddress: "DoNotReply@0b745518-72fa-4e25-b409-445ce615627e.azurecomm.net",
                    recipientAddress: email,
                    subject: "Password Reset",
                    htmlContent: $"<html><body><h2>Restablecimiento de contraseña</h2><p>Hola,</p><p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.</p><p>Por favor, haz clic en el siguiente enlace para restablecer tu contraseña:</p><p><a href=\"{resetLink}\">Click aquí para restablecer tu contraseña</a></p><p>Si no solicitaste este cambio, por favor ignora este correo.</p><p>Gracias,</p><p>Tu equipo de soporte</p></body></html>",
                    plainTextContent: $"Click the link to reset your password: {resetLink}");
            
            }catch(EmailNotSendException ex)
            {
                errors.Add(ex.Message);
            }

            if (errors.Count > 0) { throw new ValuesInvalidException(errors); }
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordDto values, CancellationToken cancellationToken)
        {
            var principal = _generatorTokens.GetPrincipalFromExpiredToken(values.token, _keyToken.GetKeyTokenEnv());

            var usernameClaimType = principal.FindFirst(ClaimTypes.Name)?.Value;

            var emailClaimType = principal.FindFirst(ClaimTypes.Email)?.Value;

            if (values.email != emailClaimType) { throw new ComparedEmailException(); }

            if (values.password != values.confirmPassword) { throw new ComparedPasswordException(); }

            List<string?> errors = new List<string?>();

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

            var account = await _bd.Accounts.FirstOrDefaultAsync(u => u.username == usernameClaimType
                                                                 && u.email == values.email, cancellationToken);

            if (account == null) { return false; }

            account.password = BCrypt.Net.BCrypt.HashPassword(values.confirmPassword);

            _validationValues.ValidationValues(account);

            _bd.Accounts.Update(account);

            if (!Save()) { throw new UnstoredValuesException(); }

            return true;
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

        private static string verifyPassword(string password)
        {
            if (password.Length < 8)
            {
                return "La contraseña debe tener al menos 8 caracteres.";
            }

            if (!RegexPatterns.RegexPatterns.Passwordregex.IsMatch(password))
            {
                return "La contraseña debe contener al menos un número, una letra minúscula o letra mayúscula y un carácter alfanumérico.";
            }

            return "";
        }
    }
}
