using AppVidaSana.Models.Dtos.Account_Profile_Dtos;

namespace AppVidaSana.ProducesResponseType.Account
{
    public class ReturnGetAccount
    {
        public string message { get; set; } = "Ok.";

        public ReturnAccountDto account { get; set; } = null!;
    }
}
