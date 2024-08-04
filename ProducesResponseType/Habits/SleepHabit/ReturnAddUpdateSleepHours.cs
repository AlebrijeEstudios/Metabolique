using AppVidaSana.Models.Dtos.Habits_Dtos;

namespace AppVidaSana.ProducesResponseType.Habits.SleepHabit
{
    public class ReturnAddUpdateSleepHours
    {
        public string message { get; set; } = "Ok.";

        public GetUpdateSleepingHoursDto sleepHours { get; set; } = null!;
    }
}
