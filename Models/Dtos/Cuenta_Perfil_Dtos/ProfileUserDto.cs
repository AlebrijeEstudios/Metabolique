using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos
{
    public class ProfileUserDto
    {
        [JsonRequired]  public Guid id { get; set; }

        [JsonRequired]  public DateOnly fechaNacimiento { get; set; }

        [JsonRequired]  public string sexo { get; set; } = null!;

        [JsonRequired]  public int estatura { get; set; }

        [JsonRequired]  public int peso { get; set; }

        [JsonRequired] public string protocolo { get; set; } = null!;
    }
}
