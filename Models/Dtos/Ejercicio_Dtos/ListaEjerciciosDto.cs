using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Ejercicio_Dtos
{
    public class ListaEjerciciosDto
    {
        [JsonRequired] public Guid idejercicio { get; set; } 

        [JsonRequired] public string tipo { get; set; } = null!;

        [JsonRequired] public string intensidad { get; set; } = null!;

        [JsonRequired] public int tiempo { get; set; }
    }
}
