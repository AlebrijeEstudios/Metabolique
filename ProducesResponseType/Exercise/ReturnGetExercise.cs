using AppVidaSana.Models.Dtos.Ejercicio_Dtos;

namespace AppVidaSana.ProducesResponseType.Exercise
{
    public class ReturnGetExercise
    {
        public string message { get; set; } = "Ok.";

        public List<ExerciseListDto> exercises { get; set; } = null!;
    }
}
