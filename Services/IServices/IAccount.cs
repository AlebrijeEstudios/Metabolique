using AppVidaSana.Models.Dtos.Account_Profile_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IAccount
    {
        Task<InfoAccountDto> GetAccount(Guid accountID, CancellationToken cancellationToken);

        Task<Guid> CreateAccount(AccountDto values, CancellationToken cancellationToken);

        Task<ProfileDto> UpdateAccount(InfoAccountDto values, CancellationToken cancellationToken);

        Task<string> DeleteAccount(Guid accountID, CancellationToken cancellationToken);

        bool Save();
    }
}
