using AppVidaSana.Models.Dtos.Account_Profile_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IProfile
    {
        void CreateProfile(Guid id, AccountDto profile);

        Task<string> UpdateProfile(ProfileDto profile);

        bool Save();
    }
}
