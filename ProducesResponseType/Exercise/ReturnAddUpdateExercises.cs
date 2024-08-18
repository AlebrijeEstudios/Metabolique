using AppVidaSana.Models.Dtos.Ejercicio_Dtos;

namespace AppVidaSana.ProducesResponseType.Exercise
{
    public class ReturnAddUpdateExercises
    {
        public bool message { get; set; } = true;

        public ExerciseListDto exercise { get; set; } = null!;
    }
}
