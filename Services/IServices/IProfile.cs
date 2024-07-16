using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IProfile
    {
        ProfileUserDto GetProfile(Guid userid);

        string CreateProfile(Guid id, ProfileUserDto profile);

        string UpdateProfile(Guid id, ProfileUserDto profile);

        bool Save();
    }
}
