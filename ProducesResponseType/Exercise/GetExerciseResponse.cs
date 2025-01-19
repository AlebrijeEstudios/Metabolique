using AppVidaSana.Models.Dtos.Exercise_Dtos;

namespace AppVidaSana.ProducesResponseType.Exercise
{
    public class GetExerciseResponse
    {
        public string message { get; set; } = "Ok.";

        public List<ExerciseDto> exercises { get; set; } = null!;

        public List<ActiveMinutesExerciseDto> activeMinutes { get; set; } = null!;

        public bool mfuStatus { get; set; }
    }
}
