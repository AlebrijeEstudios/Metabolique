using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;

namespace AppVidaSana.ProducesResponseType.Habits.MFUsHabits
{
    public class ReturnResponsesAndResultsMFUsHabits
    {
        public string message { get; set; } = "Ok.";

        public RetrieveResponsesHabitsDto mfus { get; set; } = null!;
    }
}
