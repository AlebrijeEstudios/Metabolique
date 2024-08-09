using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;

namespace AppVidaSana.ProducesResponseType.Habits.MFUsHabits
{
    public class ReturnRetrieveResponsesHabits
    {
        public bool message { get; set; } = true;

        public RetrieveResponsesHabitsDto responsesAnswers { get; set; } = null!;
    }
}
