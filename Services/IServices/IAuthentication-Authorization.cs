using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.Reset_Password_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IAuthentication_Authorization
    {
        Task<TokensDto> LoginAccountAsync(LoginDto login, CancellationToken cancellationToken);

        Task<TokensDto> RefreshTokenAsync(TokensDto values, CancellationToken cancellationToken);

        bool Save();
    }
}
