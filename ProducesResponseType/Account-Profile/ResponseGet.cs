using AppVidaSana.Models.Dtos.Account_Profile_Dtos;

namespace AppVidaSana.ProducesResponseType.Account
{
    public class ResponseGet
    {
        public string message { get; set; } = "Ok.";

        public InfoAccountDto account { get; set; } = null!;
    }
}
