using AppVidaSana.Models.Dtos.AdminWeb_Dtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWAuth
    {
        Task<TokenAdminDto> LoginAdminAsync(LoginAdminDto login, CancellationToken cancellationToken);
    }
}
