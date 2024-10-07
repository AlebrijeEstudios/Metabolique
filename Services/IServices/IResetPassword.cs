using AppVidaSana.Models.Dtos.Reset_Password_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IResetPassword
    {
        Task<TokenDto> PasswordResetToken(EmailDto email);

        Task<bool> ResetPassword(ResetPasswordDto values);

        void SendEmail(string email, string resetLink);

        bool Save();
    }
}
