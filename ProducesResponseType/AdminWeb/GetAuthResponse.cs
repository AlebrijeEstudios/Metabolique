using AppVidaSana.Models.Dtos.AdminWeb_Dtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb
{
    public class GetAuthResponse
    {
        public string message { get; set; } = "Ok.";

        public TokenAdminDto auth { get; set; } = null!;
    }
}
