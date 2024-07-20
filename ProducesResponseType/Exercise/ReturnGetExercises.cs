using AppVidaSana.Models.Dtos.Ejercicio_Dtos;

namespace AppVidaSana.ProducesResponseType.Exercise
{
    public class ReturnGetExercises
    {
        public string message { get; set; } = "Ok.";

        public List<ExerciseListDto> response { get; set; } = null!;
    }
}
