using AppVidaSana.Models.Dtos.Account_Profile_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IProfile
    {
        void CreateProfileAsync(Guid accountID, AccountDto values, CancellationToken cancellationToken);

        Task<string> UpdateProfileAsync(ProfileDto values, CancellationToken cancellationToken);

        bool Save();
    }
}
