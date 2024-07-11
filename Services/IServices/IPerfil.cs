using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IPerfil
    {
        ProfileUserDto GetProfile(Guid userid);

        string CreateProfile(Guid id, ProfileUserDto profile);

        string UpdateProfile(Guid id, ProfileUserDto profiledto);

        bool Guardar();
    }
}
