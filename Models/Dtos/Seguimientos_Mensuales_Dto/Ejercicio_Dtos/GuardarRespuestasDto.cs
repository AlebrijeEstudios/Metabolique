using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos
{
    public class GuardarRespuestasDto
    {
        [JsonRequired] public Guid id { get; set; }

        [JsonRequired] public string mes { get; set; } = null!;

        [JsonRequired] public int año { get; set; }

        [JsonRequired] public int pregunta1 { get; set; }

        [JsonRequired] public int pregunta2 { get; set; }

        [JsonRequired] public int pregunta3 { get; set; }

        [JsonRequired] public int pregunta4 { get; set; }

        [JsonRequired] public int pregunta5 { get; set; }

        [JsonRequired] public int pregunta6 { get; set; }

        [JsonRequired] public int pregunta7 { get; set; }
    }
}
