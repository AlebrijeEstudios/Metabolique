using AppVidaSana.Data;
using AppVidaSana.Services.IServices;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using Azure.Communication.Email;
using Azure;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AppVidaSana.Models;
using System.ComponentModel.DataAnnotations;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Exceptions;

namespace AppVidaSana.Services
{
    public class AccountService : IAccount
    {
        private readonly AppDbContext _bd;
        private readonly string keyToken;

        public AccountService(AppDbContext bd)
        {
            _bd = bd;
            keyToken = Environment.GetEnvironmentVariable("TOKEN") ?? Environment.GetEnvironmentVariable("TOKEN_Replacement");
        }

        public CreateAccountReturn CreateAccount(CreateAccountProfileDto account)
        {
            List<string?> er = new List<string?>();

            string message = "";

            string vUsername = verifyUsername(account.username); 
            
            if(vUsername != ""){ er.Add(vUsername); }

            try
            {
                string vEmail = verifyEmail(account.email);
                
                if(vEmail != ""){ er.Add(vEmail); }

            }catch(EmailValidationTimeoutException ex)
            {
                message =  ex.Message;
            }

            try
            {
                string vPassword = verifyPassword(account.password);

                if (vPassword != "") { er.Add(vPassword); }

            }catch(PasswordValidationTimeoutException ex)
            {
                message =  ex.Message;
            }

            if(er.Count > 0) 
            {
                throw new ValuesInvalidException(er);
            }
            
            Account us = new Account
            {
                username = account.username,
                email = account.email,
                password = BCrypt.Net.BCrypt.HashPassword(account.password),
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(us, null, null);

            if (!Validator.TryValidateObject(us, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }

            _bd.Accounts.Add(us);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            var user = _bd.Accounts.AsEnumerable()
            .FirstOrDefault(u => string.Equals(u.email, account.email, StringComparison.OrdinalIgnoreCase));

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            CreateAccountReturn result = new CreateAccountReturn
            {
                accountID = user.accountID,
                messageException = message
            };

            return result;

        }

        public ReturnAccountDto GetAccount(Guid accountid)
        {
            var account = _bd.Accounts.Find(accountid);
            var profile = _bd.Profiles.Find(accountid);

            if (account == null || profile == null)
            {
                throw new UserNotFoundException();
            }

            ReturnAccountDto infoAccount = new ReturnAccountDto
            {
                accountID = account.accountID,
                username = account.username,
                email = account.email,
                birthDate = profile.birthDate,
                sex = profile.sex,
                stature = profile.stature,
                weight = profile.weight,
                protocolToFollow = profile.protocolToFollow
            };

            return infoAccount;
        }

        public TokenUserDto LoginAccount(LoginAccountDto login)
        {
            var user = _bd.Accounts.AsEnumerable()
            .FirstOrDefault(u => string.Equals(u.email, login.email, StringComparison.OrdinalIgnoreCase));


            if (user == null || !BCrypt.Net.BCrypt.Verify(login.password, user.password))
            {
                throw new LoginException();
            }

            var tok = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(keyToken);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, user.username.ToString()),
                        new Claim(ClaimTypes.Email, user.email.ToString()),
                        new Claim(ClaimTypes.Role, user.role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = "metaboliqueapi",
                Audience = "metabolique.com",
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tok.CreateToken(tokenDescriptor);
            var account = _bd.Accounts.FirstOrDefault(u => u.email == login.email);

            if (account == null)
            {
                throw new UserNotFoundException();
            }

            TokenUserDto ut = new TokenUserDto()
            {
                token = tok.WriteToken(token),
                accountID = account.accountID
            };

            return ut;
        }

        public ReturnProfileDto UpdateAccount(ReturnAccountDto infoAccount)
        {
            var user = _bd.Accounts.Find(infoAccount.accountID);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            if(user.username != infoAccount.username)
            {
               user.username = infoAccount.username;
            }

            if (user.email != infoAccount.email)
            {
                user.email = infoAccount.email;
            }

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(user, null, null);

            if (!Validator.TryValidateObject(user, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }

            _bd.Accounts.Update(user);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            ReturnProfileDto result = new ReturnProfileDto
            {
                accountID = infoAccount.accountID,
                birthDate = infoAccount.birthDate,
                sex = infoAccount.sex,
                stature = infoAccount.stature,
                weight = infoAccount.weight,
                protocolToFollow = infoAccount.protocolToFollow
            };

            return result;
        }

        public bool ResetPassword(ResetPasswordDto model)
        {

            if (model.password != model.confirmPassword)
            {
                throw new ComparedPasswordException();
            }

            string vPassword = verifyPassword(model.confirmPassword);

            List<string?> er = new List<string?>();

            if (vPassword != "")
            {
                er.Add(vPassword);
            }

            if (er.Count > 0)
            {
                throw new ValuesInvalidException(er);
            }

            var principal = GetPrincipalFromExpiredToken(model.token);

            if (principal == null)
            {
                return false;
            }

            var user = _bd.Accounts.FirstOrDefault(u => u.email == model.email);

            if (user == null)
            {
                return false;
            }

            user.password = BCrypt.Net.BCrypt.HashPassword(model.confirmPassword);

            _bd.Accounts.Update(user);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return true;
        }

        public string DeleteAccount(Guid userid)
        {
            var user = _bd.Accounts.Find(userid);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            _bd.Accounts.Remove(user);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return "El usuario ha sido eliminado correctamente.";
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

        public TokenUserDto RequestPasswordResetToken(ForgotPasswordDto request)
        {
            var user = _bd.Accounts.FirstOrDefault(u => u.email == request.email);

            if (user == null)
            {
                throw new EmailNotSendException();
            }

            var tok = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(keyToken);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, user.username.ToString()),
                        new Claim(ClaimTypes.Email, user.email.ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = "metaboliqueapi",
                Audience = "metabolique.com",
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tok.CreateToken(tokenDescriptor);

            TokenUserDto ut = new TokenUserDto()
            {
                token = tok.WriteToken(token),
                accountID = user.accountID

            };

            return ut;
        }

        public void SendPasswordResetEmail(string email, string resetLink)
        {
            EmailClient emailClient = new EmailClient(Environment.GetEnvironmentVariable("EMAIL_API"));
            emailClient.Send(
                WaitUntil.Completed,
                senderAddress: "DoNotReply@cdc6f1d4-e706-4ef8-8d96-fe23fb30a815.azurecomm.net",
                recipientAddress: email,
                subject: "Password Reset",
                htmlContent: $"<html><body><h2>Restablecimiento de contraseña</h2><p>Hola,</p><p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.</p><p>Por favor, haz clic en el siguiente enlace para restablecer tu contraseña:</p><p><a href=\"{resetLink}\">Click aquí para restablecer tu contraseña</a></p><p>Si no solicitaste este cambio, por favor ignora este correo.</p><p>Gracias,</p><p>Tu equipo de soporte</p></body></html>",
                plainTextContent: $"Click the link to reset your password: {resetLink}");

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

        private string verifyUsername(string username)
        {
            var existingAccount = _bd.Accounts.FirstOrDefault(c => c.username == username);
            if (existingAccount != null)
            {
                return "Este nombre de usuario ya está en uso.";
            }

            return "";
        }

        private string verifyEmail(string email)
        {
            if (!RegexPatterns.RegexPatterns.Emailregex.IsMatch(email))
            {
                return "El correo electrónico no tiene un formato válido.";
            }

            var existingEmail = _bd.Accounts.FirstOrDefault(c => c.email == email);
            if (existingEmail != null)
            {
                return "Este correo electrónico está ligado a una cuenta existente.";
            }

            return "";
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
