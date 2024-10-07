using AppVidaSana.Models.Dtos.Reset_Password_Dtos;

namespace AppVidaSana.ProducesResponseType.Authenticator
{
    public class ResponseLogin
    {
        public string message { get; set; } = "Ok.";

        public TokenDto auth { get; set; } = null!;
    }
}
