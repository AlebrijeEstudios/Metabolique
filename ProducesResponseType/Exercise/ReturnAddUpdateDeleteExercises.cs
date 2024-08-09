using AppVidaSana.Models.Dtos.Ejercicio_Dtos;

namespace AppVidaSana.ProducesResponseType.Exercise
{
    public class ReturnAddUpdateDeleteExercises
    {
        public bool message { get; set; } = true;

        public List<ExerciseListDto> exercises { get; set; } = null!;
    }
}
