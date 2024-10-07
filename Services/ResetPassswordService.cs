using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Reset_Password_Dtos;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;
using Azure;
using Azure.Communication.Email;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AppVidaSana.Services
{
    public class ResetPassswordService : IResetPassword
    {
        private readonly AppDbContext _bd;
        private readonly string keyToken;

        public ResetPassswordService(AppDbContext bd)
        {
            _bd = bd;
            keyToken = Environment.GetEnvironmentVariable("TOKEN") ?? Environment.GetEnvironmentVariable("TOKEN_Replacement");
        }

        public async Task<bool> ResetPassword(ResetPasswordDto values)
        {
            if (values.password != values.confirmPassword) { throw new ComparedPasswordException(); }

            List<string?> errors = new List<string?>();

            string message = "";

            try
            {
                string verifyStatusPassword = VerifyValues.verifyPassword(values.password);

                if (verifyStatusPassword != "") { errors.Add(verifyStatusPassword); }

            }
            catch (PasswordValidationTimeoutException ex)
            {
                message = ex.Message;
                errors.Add(message);
            }

            if (errors.Count > 0) { throw new ValuesInvalidException(errors); }

            var claimPrincipal = GetPrincipalFromExpiredToken(values.token);

            if (claimPrincipal == null) { return false; }

            var account = await _bd.Accounts.FirstOrDefaultAsync(u => u.email == values.email);

            if (account == null) { return false; }

            account.password = BCrypt.Net.BCrypt.HashPassword(values.confirmPassword);

            _bd.Accounts.Update(account);

            if (!Save()) { throw new UnstoredValuesException(); }

            return true;
        }

        public async Task<TokenDto> PasswordResetToken(EmailDto value)
        {
            var account = await _bd.Accounts.FirstOrDefaultAsync(u => u.email == value.email);

            if (account == null) { throw new EmailNotSendException(); }

            var tok = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(keyToken);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, account.username.ToString()),
                        new Claim(ClaimTypes.Email, account.email.ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = "metaboliqueapi",
                Audience = "metabolique.com",
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tok.CreateToken(tokenDescriptor);

            TokenDto ut = new TokenDto()
            {
                token = tok.WriteToken(token),
                accountID = account.accountID

            };

            return ut;
        }

        public async void SendEmail(string email, string resetLink)
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

            if (errors.Any()) { throw new ValuesInvalidException(errors); }
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

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes(keyToken);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "metaboliqueapi",
                ValidAudience = "metabolique.com",
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateLifetime = false
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken;
            var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out securityToken);

            var jwtSecurityToken = securityToken as JwtSecurityToken;

            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new SecurityTokenException("Invalid token");
            }

            return principal;
        }
    }
}
