using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Graphics_Dtos;

namespace AppVidaSana.ProducesResponseType.Exercise
{
    public class ReturnGetExercise
    {
        public string message { get; set; } = "Ok.";

        public List<ExerciseListDto> exercises { get; set; } = null!;

        public List<GraphicValuesExerciseDto> activeMinutes { get; set; } = null!;

        public bool mfuStatus { get; set; }
    }
}
