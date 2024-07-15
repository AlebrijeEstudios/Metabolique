using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos
{
    public class LoginUserDto
    {
        [JsonRequired]  public string email { get; set; } = null!;

        [JsonRequired]  public string password { get; set; } = null!;

    }
}
