using AppVidaSana.Models.Dtos.Habits_Dtos.Sleep;

namespace AppVidaSana.ProducesResponseType.Habits.SleepHabit
{
    public class ResponseSleepHabit
    {
        public string message { get; set; } = "Ok.";

        public SleepHabitInfoDto sleepHabit { get; set; } = null!;
    }
}
