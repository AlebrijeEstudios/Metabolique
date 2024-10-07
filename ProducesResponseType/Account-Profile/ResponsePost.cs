using AppVidaSana.Models.Dtos.Reset_Password_Dtos;

namespace AppVidaSana.ProducesResponseType.Account
{
    public class ResponsePost
    {
        public string message { get; set; } = "Ok.";

        public TokenDto auth { get; set; } = null!;
    }
}
