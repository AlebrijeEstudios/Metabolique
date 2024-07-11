using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos
{
    public class TokenUserDto
    {
        public string Token { get; set; } = null!;

        public Guid id { get; set; }
    }
}
