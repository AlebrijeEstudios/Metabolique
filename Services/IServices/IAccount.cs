using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IAccount
    {
        ReturnAccountDto GetAccount(Guid accountid);

        Guid CreateAccount(CreateAccountProfileDto account);

        ReturnProfileDto UpdateAccount(ReturnAccountDto infoAccount);

        string DeleteAccount(Guid userid);

        Task<TokenUserDto> LoginAccount(LoginAccountDto login, CancellationToken cancellationToken);

        TokenUserDto RequestPasswordResetToken(ForgotPasswordDto request);

        bool ResetPassword(ResetPasswordDto model);

        void SendPasswordResetEmail(string email, string resetLink);

        bool Save();
    }
}
