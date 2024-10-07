using AppVidaSana.Models.Dtos.Account_Profile_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IAccount
    {
        Task<InfoAccountDto> GetAccount(Guid accountID);

        Task<Guid> CreateAccount(AccountDto values);

        Task<ProfileDto> UpdateAccount(InfoAccountDto values);

        Task<string> DeleteAccount(Guid accountID);

        bool Save();
    }
}
