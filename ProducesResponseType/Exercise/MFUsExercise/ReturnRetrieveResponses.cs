using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;

namespace AppVidaSana.ProducesResponseType.Exercise.MFUsExercise
{
    public class ReturnRetrieveResponses
    {
        public string message { get; set; } = "Ok.";

        public RetrieveResponsesDto response { get; set; } = null!;
    }
}

