using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;

namespace AppVidaSana.ProducesResponseType.Exercise.MFUsExercise
{
    public class ReturnResponsesAndResultsMFUsExercise
    {
        public bool message { get; set; } = true;

        public RetrieveResponsesExerciseDto mfus { get; set; } = null!;
    }
}
