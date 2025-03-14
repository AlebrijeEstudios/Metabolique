﻿using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.Models.Dtos.Reset_Password_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IAuthenticationAuthorization
    {
        Task<TokensDto> LoginAccountAsync(LoginDto login, CancellationToken cancellationToken);

        Task<TokenAdminDto> LoginAdminAsync(LoginAdminDto login, CancellationToken cancellationToken);

        Task<string> LogoutAccountAsync(Guid accountID, CancellationToken cancellationToken);

        Task<TokensDto> RefreshTokenAsync(TokensDto values, CancellationToken cancellationToken);
    }
}
