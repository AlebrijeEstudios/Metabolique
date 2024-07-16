using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos
{
    public class TokenUserDto
    {
        [JsonRequired]  public string token { get; set; } = null!;

        [JsonRequired]  public Guid accountID { get; set; }
    }
}
