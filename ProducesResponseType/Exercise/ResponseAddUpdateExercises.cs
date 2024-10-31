using AppVidaSana.Models.Dtos.Ejercicio_Dtos;

namespace AppVidaSana.ProducesResponseType.Exercise
{
    public class ResponseAddUpdateExercises
    {
        public string message { get; set; } = "Ok.";

        public ExerciseDto exercise { get; set; } = null!;
    }
}
