using AppVidaSana.Models.Dtos.Habits_Dtos;

namespace AppVidaSana.ProducesResponseType.Habits.DrugsHabit
{
    public class ReturnAddUpdateSleepHoursAndDrugsConsumed
    {
        public string message { get; set; } = "Ok.";

        public ReturnSleepHoursAndDrugsConsumedDto habitSleepDrugs { get; set; } = null!;
    }
}
