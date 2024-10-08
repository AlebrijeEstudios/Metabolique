using AppVidaSana.Models.Dtos.Reset_Password_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IResetPassword
    {
        Task<TokenDto> PasswordResetToken(EmailDto value, CancellationToken cancellationToken);

        Task<bool> ResetPassword(ResetPasswordDto values, CancellationToken cancellationToken);

        void SendEmail(string email, string resetLink);

        bool Save();
    }
}
