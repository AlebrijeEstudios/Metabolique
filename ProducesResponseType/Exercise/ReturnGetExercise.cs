using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Graphics_Dtos;

namespace AppVidaSana.ProducesResponseType.Exercise
{
    public class ReturnGetExercise
    {
        public bool message { get; set; } = true;

        public List<ExerciseListDto> exercises { get; set; } = null!;

        public List<GraphicsValuesExerciseDto> activeMinutes { get; set; } = null!;
    }
}
