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
using System.Text.RegularExpressions;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;

namespace AppVidaSana.Services
{
    public class CuentaService : ICuenta
    {
        private readonly AppDbContext _bd;
        private readonly string clave;

        public CuentaService(AppDbContext bd)
        {
            _bd = bd;
            clave = Environment.GetEnvironmentVariable("TOKEN") ?? "ABCD67890_secure_key_32_characters";

        }

        public string CreateUser(RegisterUserDto user)
        {
            if (user == null)
            {
                return "No se guardaron los datos, intentelo de nuevo";
            }

            string vUsername = verificarUsername(user.username);
            string vCorreo = verificarCorreo(user.email);
            string vPassword = verificarPassword(user.password);

            List<string?> er = new List<string?>();

            if(vUsername != "")
            {
                er.Add(vUsername);
            }

            if(vCorreo != "")
            {
                er.Add(vCorreo);
            }

            if(vPassword != "")
            {
                er.Add(vPassword);
            }

            if(er.Count > 0) 
            {
                throw new ValuesInvalidException(er);
            }
            
            Cuenta us = new Cuenta
            {
                username = user.username,
                email = user.email,
                password = BCrypt.Net.BCrypt.HashPassword(user.password),
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

            _bd.Cuentas.Add(us);
            Guardar();
            return "Los datos han sido guardados correctamente";

        }

        public Cuenta GetUser(Guid userid)
        {
            var user = _bd.Cuentas.Find(userid);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            return user;
        }

        public TokenUserDto LoginUser(LoginUserDto userDTO)
        {
            var user = _bd.Cuentas.AsEnumerable()
            .FirstOrDefault(u => string.Equals(u.email, userDTO.email, StringComparison.OrdinalIgnoreCase));


            if (user == null || !BCrypt.Net.BCrypt.Verify(userDTO.password, user.password))
            {
                throw new LoginException();
            }

            var tok = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(clave);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, user.username.ToString()),
                        new Claim(ClaimTypes.Email, user.email.ToString()),
                        new Claim(ClaimTypes.Role, user.role)
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                Issuer = "vidasanaapi",
                Audience = "vidasana.com",
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tok.CreateToken(tokenDescriptor);
            var uid = _bd.Cuentas.FirstOrDefault(u => u.email == userDTO.email);

            if (uid == null)
            {
                throw new UserNotFoundException();
            }

            TokenUserDto ut = new TokenUserDto()
            {
                Token = tok.WriteToken(token),
                id = uid.cuentaID
            };

            return ut;

        }
        public string UpdateUser(Guid id, UserInfoDto userdto)
        {
            var user = _bd.Cuentas.Find(id);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            string vUsername = verificarUsername(userdto.username);
            string vCorreo = verificarCorreo(userdto.email);

            List<string?> er = new List<string?>();

            if (vUsername != "")
            {
                er.Add(vUsername);
            }

            if (vCorreo != "")
            {
                er.Add(vCorreo);
            }


            if (er.Count > 0)
            {
                throw new ValuesInvalidException(er);
            }

            user.username = userdto.username;
            user.email = userdto.email;

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

            _bd.Cuentas.Update(user);
            Guardar();
            return "Actualización completada";
        }

        public string DeleteUser(Guid userid)
        {
            var user = _bd.Cuentas.Find(userid);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            _bd.Cuentas.Remove(user);
            Guardar();
            return "El usuario a sido eliminado correctamente.";
        }

        public bool Guardar()
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
            var user = _bd.Cuentas.FirstOrDefault(u => u.email == request.email);

            if (user == null)
            {
                throw new EmailNotSendException();
            }

            var tok = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(clave);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, user.username.ToString()),
                        new Claim(ClaimTypes.Email, user.email.ToString()),
                }),
                Expires = DateTime.UtcNow.AddHours(1),
                Issuer = "vidasanaapi",
                Audience = "vidasana.com",
                SigningCredentials = new(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tok.CreateToken(tokenDescriptor);

            TokenUserDto ut = new TokenUserDto()
            {
                Token = tok.WriteToken(token),
                id = user.cuentaID

            };

            return ut;
        }

        public void SendPasswordResetEmail(string email, string resetLink)
        {
            EmailClient emailClient = new EmailClient(Environment.GetEnvironmentVariable("KEY_API"));
            emailClient.Send(
                WaitUntil.Completed,
                senderAddress: "DoNotReply@cdc6f1d4-e706-4ef8-8d96-fe23fb30a815.azurecomm.net",
                recipientAddress: email,
                subject: "Password Reset",
                htmlContent: $"<html><body><h2>Restablecimiento de contraseña</h2><p>Hola,</p><p>Hemos recibido una solicitud para restablecer la contraseña de tu cuenta.</p><p>Por favor, haz clic en el siguiente enlace para restablecer tu contraseña:</p><p><a href=\"{resetLink}\">Click aquí para restablecer tu contraseña</a></p><p>Si no solicitaste este cambio, por favor ignora este correo.</p><p>Gracias,</p><p>Tu equipo de soporte</p></body></html>",
                plainTextContent: $"Click the link to reset your password: {resetLink}");

        }

        public bool ResetPassword(ResetPasswordDto model)
        {
            
            if(!(model.password == model.confirmpassword))
            {
                throw new ComparedPasswordException();
            }

            string? vPassword = verificarPassword(model.confirmpassword);

            List<string?> er = new List<string?>();

            if(vPassword != "")
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

            var user = _bd.Cuentas.FirstOrDefault(u => u.email == model.email);

            if (user == null)
            {
                return false;
            }

            user.password = BCrypt.Net.BCrypt.HashPassword(model.confirmpassword);

            _bd.Cuentas.Update(user);
            Guardar();

            return true;
        }

        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var key = Encoding.ASCII.GetBytes(clave);
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "vidasanaapi",
                ValidAudience = "vidasana.com",
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

        private string verificarUsername(string usernameSinVerificar)
        {
            var cuentaExistente = _bd.Cuentas.FirstOrDefault(c => c.username == usernameSinVerificar);
            if (cuentaExistente != null)
            {
                return "Este nombre de usuario ya está en uso";
            }

            return "";
        }

        private string verificarCorreo(string correoSinVerificar)
        {
            Regex regex = new Regex(@"[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9_-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}");

            if (!regex.IsMatch(correoSinVerificar))
            {
                return "El correo electrónico no tiene un formato válido";
            }

            var correoExistente = _bd.Cuentas.FirstOrDefault(c => c.email == correoSinVerificar);
            if (correoExistente != null)
            {
                return "Este correo electrónico está ligado a una cuenta existente";
            }

            return "";
        }

        private string verificarPassword(string contraseñaSinVerificar)
        {
            Regex patronGeneral = new Regex(@"(?=.*[a-zA-Z])(?=.*\d)(?=.*[!""#$%&'()*+,-./:;=?@[\]^_`{|}~])[\w!""#$%&'()*+,-./:;=?@[\]^_`{|}~]");

            if (!patronGeneral.IsMatch(contraseñaSinVerificar))
            {
                return "La contraseña debe contener al menos un número, una letra minúscula o letra mayúscula y un carácter alfanumérico";
            }

            return "";
        }
    }
}
