using AppVidaSana.Models.Dtos.Ejercicio_Dtos;

namespace AppVidaSana.ProducesResponseType.Exercise
{
    public class ReturnAddUpdateExercises
    {
        public string message { get; set; } = "Ok.";

        public ExerciseListDto exercise { get; set; } = null!;
    }
}
