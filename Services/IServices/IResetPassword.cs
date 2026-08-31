using AppVidaSana.Models.Dtos.Reset_Password_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IResetPassword
    {
        Task<string> PasswordResetTokenAsync(EmailDto value, CancellationToken cancellationToken);

        Task<bool> ResetPasswordAsync(ResetPasswordDto values, CancellationToken cancellationToken);

        Task SendEmailAsync(string email, string resetLink);

        bool Save();
    }
}
