using AppVidaSana.Models.Dtos.Account_Profile_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IAccount
    {
        Task<InfoAccountDto> GetAccountAsync(Guid accountID, CancellationToken cancellationToken);

        Task<List<InfoAccountDto>> GetPatientsAsync(Guid doctorID, int page, CancellationToken cancellationToken);

        Task<Guid> CreateAccountAsync(AccountDto values, CancellationToken cancellationToken);

        Task<ProfileDto> UpdateAccountAsync(InfoAccountDto values, CancellationToken cancellationToken);

        Task<string> DeleteAccountAsync(Guid accountID, CancellationToken cancellationToken);

        bool Save();
    }
}
