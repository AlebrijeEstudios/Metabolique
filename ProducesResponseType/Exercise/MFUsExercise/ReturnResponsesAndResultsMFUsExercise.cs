using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Exercise_Dtos;

namespace AppVidaSana.ProducesResponseType.Exercise.MFUsExercise
{
    public class ReturnResponsesAndResultsMFUsExercise
    {
        public string message { get; set; } = "Ok.";

        public RetrieveResponsesExerciseDto? mfus { get; set; } = null!;
    }
}
