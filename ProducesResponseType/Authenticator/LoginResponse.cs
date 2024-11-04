using AppVidaSana.Models.Dtos.Reset_Password_Dtos;

namespace AppVidaSana.ProducesResponseType.Authenticator
{
    public class LoginResponse
    {
        public string message { get; set; } = "Ok.";

        public TokensDto auth { get; set; } = null!;
    }
}
