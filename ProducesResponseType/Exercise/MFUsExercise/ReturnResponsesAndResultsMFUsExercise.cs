using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;

namespace AppVidaSana.ProducesResponseType.Exercise.MFUsExercise
{
    public class ReturnResponsesAndResultsMFUsExercise
    {
        public string message { get; set; } = "Ok.";

        public RetrieveResponsesExerciseDto? mfus { get; set; } = null!;
    }
}
