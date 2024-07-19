using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IProfile
    {
        bool CreateProfile(Guid id, CreateAccountProfileDto profile);

        string UpdateProfile(Guid id, ProfileUserDto profile);

        bool Save();
    }
}
