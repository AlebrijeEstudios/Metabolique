using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;

namespace AppVidaSana.ProducesResponseType.Account
{
    public class ReturnCreateAccount
    {
        public bool message { get; set; } = true;

        public TokenUserDto auth { get; set; } = null!;
    }
}
