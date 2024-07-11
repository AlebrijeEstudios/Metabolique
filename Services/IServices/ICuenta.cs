using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface ICuenta
    {
        Cuenta GetUser(Guid userid);

        string CreateUser(RegisterUserDto user);

        string UpdateUser(Guid id, UserInfoDto userdto);

        string DeleteUser(Guid userid);

        TokenUserDto RequestPasswordResetToken(ForgotPasswordDto request);

        TokenUserDto LoginUser(LoginUserDto userDTO);

        bool ResetPassword(ResetPasswordDto model);

        void SendPasswordResetEmail(string email, string resetLink);

        bool Guardar();
    }
}
