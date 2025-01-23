using AppVidaSana.Models.Dtos.Exercise_Dtos;
using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Exercise_Dtos
{
    public class InfoGeneralExerciseDto
    {
        [JsonRequired] public List<ExerciseDto> exercises { get; set; } = null!;

        [JsonRequired] public List<ActiveMinutesExerciseDto> activeMinutes { get; set; } = null!;

        [JsonRequired] public bool mfuStatus { get; set; }

    }
}
