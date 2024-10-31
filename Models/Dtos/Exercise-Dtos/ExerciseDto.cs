using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Ejercicio_Dtos
{
    public class ExerciseDto
    {
        [JsonRequired] public Guid exerciseID { get; set; }

        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public DateOnly dateExercise { get; set; }

        [JsonRequired] public string typeExercise { get; set; } = null!;

        [JsonRequired] public string intensityExercise { get; set; } = null!;

        [JsonRequired] public int timeSpent { get; set; }
    }
}
