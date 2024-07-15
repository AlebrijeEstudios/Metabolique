using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos
{
    public class TokenUserDto
    {
        [JsonRequired]  public string Token { get; set; } = null!;

        [JsonRequired]  public Guid id { get; set; }
    }
}
