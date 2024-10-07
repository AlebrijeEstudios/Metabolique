using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.Reset_Password_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IAuthentication_Authorization
    {
        Task<TokenDto> LoginAccount(LoginDto login, CancellationToken cancellationToken);
    }
}
