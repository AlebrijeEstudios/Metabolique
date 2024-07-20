using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;

namespace AppVidaSana.ProducesResponseType.Authenticator
{
    public class ReturnLoginAccount
    {
        public string message { get; set; } = "Ok.";

        public TokenUserDto response { get; set; } = null!;
    }
}
