using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IAccount
    {
        AccountInfoDto GetAccount(Guid accountid);

        string CreateAccount(RegisterUserDto account);

        string UpdateAccount(Guid id, AccountInfoDto infoAccount);

        string DeleteAccount(Guid userid);

        TokenUserDto RequestPasswordResetToken(ForgotPasswordDto request);

        TokenUserDto LoginAccount(LoginAccountDto login);

        bool ResetPassword(ResetPasswordDto model);

        void SendPasswordResetEmail(string email, string resetLink);

        bool Save();
    }
}
