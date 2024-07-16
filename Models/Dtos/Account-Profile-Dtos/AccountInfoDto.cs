using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos
{
    public class AccountInfoDto
    {
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public string username { get; set; } = null!;

        [JsonRequired] public string email { get; set; } = null!;

    }
}
