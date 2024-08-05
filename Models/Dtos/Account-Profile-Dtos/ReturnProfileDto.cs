using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos
{
    public class ReturnProfileDto
    {
        [JsonRequired]  public Guid accountID { get; set; }

        [JsonRequired]  public DateOnly birthDate { get; set; }

        [JsonRequired]  public string sex { get; set; } = null!;

        [JsonRequired]  public float stature { get; set; }

        [JsonRequired]  public float weight { get; set; }

        [JsonRequired] public string protocolToFollow { get; set; } = null!;
    }
}
