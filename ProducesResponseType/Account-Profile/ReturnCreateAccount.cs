using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;

namespace AppVidaSana.ProducesResponseType.Account
{
    public class ReturnCreateAccount
    {
        public string message { get; set; } = "Ok.";

        public TokenUserDto response { get; set; } = null!;
    }
}
