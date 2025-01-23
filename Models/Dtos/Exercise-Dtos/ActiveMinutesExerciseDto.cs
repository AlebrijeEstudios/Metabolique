using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Exercise_Dtos
{
    public class ActiveMinutesExerciseDto
    {
        [JsonRequired] public DateOnly date { get; set; } 

        [JsonRequired] public int value { get; set; }

    }
}
