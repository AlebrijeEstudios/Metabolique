using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Graphics_Dtos
{
    public class ActiveMinutesExerciseDto
    {
        [JsonRequired] public DateOnly date { get; set; } 

        [JsonRequired] public int value { get; set; }

    }
}
