using AppVidaSana.Models.Dtos.Habits_Dtos;

namespace AppVidaSana.ProducesResponseType.Habits.SleepHabit
{
    public class ReturnGetSleepingHours
    {
        public string message { get; set; } = "Ok.";

        public List<GetSleepingHoursDto> hoursSleep { get; set; } = null!;
    }
}
