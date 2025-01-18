using AppVidaSana.Models.Dtos.Exercise_Dtos;

namespace AppVidaSana.ProducesResponseType.Exercise
{
    public class AddUpdateExercisesResponse
    {
        public string message { get; set; } = "Ok.";

        public ExerciseDto exercise { get; set; } = null!;
    }
}
