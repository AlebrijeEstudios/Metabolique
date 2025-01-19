using AppVidaSana.Models.Dtos.Account_Profile_Dtos;

namespace AppVidaSana.ProducesResponseType.Account_Profile
{
    public class AccountResponse
    {
        public string message { get; set; } = "Ok.";

        public InfoAccountDto account { get; set; } = null!;
    }
}
