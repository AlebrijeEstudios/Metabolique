using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Graphics_Dtos;

namespace AppVidaSana.ProducesResponseType.Exercise
{
    public class ReturnGetValuesGraphic
    {
        public string message { get; set; } = "Ok.";

        public List<GExerciseDto> response { get; set; } = null!;
    }
}
