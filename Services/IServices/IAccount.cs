using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IAccount
    {
        ReturnAccountDto GetAccount(Guid accountid);

        CreateAccountReturn CreateAccount(CreateAccountProfileDto account);

        ProfileUserDto UpdateAccount(Guid id, CreateAccountProfileDto infoAccount);

        string DeleteAccount(Guid userid);

        TokenUserDto RequestPasswordResetToken(ForgotPasswordDto request);

        TokenUserDto LoginAccount(LoginAccountDto login);

        bool ResetPassword(ResetPasswordDto model);

        void SendPasswordResetEmail(string email, string resetLink);

        bool Save();
    }
}
