using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos
{
    public class ForgotPasswordDto
    {
        [JsonRequired]  public string email { get; set; } = null!;
    }
}
