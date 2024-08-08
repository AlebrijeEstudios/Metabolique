using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;

namespace AppVidaSana.ProducesResponseType.Exercise.MFUsExercise
{
    public class ReturnRetrieveResponsesExercise
    {
        public string message { get; set; } = "Ok.";

        public bool actionStatus { get; set; } = true;

        public RetrieveResponsesExerciseDto responsesAnswers { get; set; } = null!;
    }
}

