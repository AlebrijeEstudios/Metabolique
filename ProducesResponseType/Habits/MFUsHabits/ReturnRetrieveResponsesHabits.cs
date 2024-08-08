using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;

namespace AppVidaSana.ProducesResponseType.Habits.MFUsHabits
{
    public class ReturnRetrieveResponsesHabits
    {
        public string message { get; set; } = "Ok.";

        public bool actionStatus { get; set; } = true;

        public RetrieveResponsesHabitsDto responsesAnswers { get; set; } = null!;
    }
}
