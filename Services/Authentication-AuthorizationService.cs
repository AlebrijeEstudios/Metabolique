using AppVidaSana.Data;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.Reset_Password_Dtos;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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
            keyToken = Environment.GetEnvironmentVariable("TOKEN") ?? Environment.GetEnvironmentVariable("TOKEN_Replacement");
            _validationValues = new ValidationValuesDB();
        }

        public async Task<TokenDto> LoginAccount(LoginDto login, CancellationToken cancellationToken)
        {
            var user = await _bd.Accounts.FirstOrDefaultAsync(u =>
                                          u.email.ToLower() == login.email.ToLower(), cancellationToken);

            if (user == null || !BCrypt.Net.BCrypt.Verify(login.password, user.password))
            {
                throw new FailLoginException();
            }

            var role = await _bd.Roles.FirstOrDefaultAsync(e => e.roleID == user.roleID);

            if (role == null) { throw new NoRoleAssignmentException(); }

            var tok = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(keyToken);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                        new Claim(ClaimTypes.Name, user.username.ToString()),
                        new Claim(ClaimTypes.Email, user.email.ToString()),
                        new Claim(ClaimTypes.Role, role.role)
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

            TokenDto ut = new TokenDto()
            {
                token = tok.WriteToken(token),
                accountID = account.accountID
            };

            return ut;
        }
    }
}
