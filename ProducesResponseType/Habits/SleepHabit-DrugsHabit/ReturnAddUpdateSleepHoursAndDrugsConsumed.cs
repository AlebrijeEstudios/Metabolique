using AppVidaSana.Models.Dtos.Habits_Dtos;

namespace AppVidaSana.ProducesResponseType.Habits.DrugsHabit
{
    public class ReturnAddUpdateSleepHoursAndDrugsConsumed
    {
        public bool message { get; set; } = true;

        public ReturnSleepHoursAndDrugsConsumedDto habitSleepDrugs { get; set; } = null!;
    }
}
