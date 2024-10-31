using AppVidaSana.Models.Dtos.Reset_Password_Dtos;

namespace AppVidaSana.ProducesResponseType.Account
{
    public class ResponseAuth
    {
        public string message { get; set; } = "Ok.";

        public TokensDto auth { get; set; } = null!;
    }
}
