using AppVidaSana.Models.Dtos.Habits_Dtos.Drugs;

namespace AppVidaSana.ProducesResponseType.Habits.DrugsHabit
{
    public class ResponseDrugsHabit
    {
        public string message { get; set; } = "Ok.";

        public DrugsHabitInfoDto drugsHabit { get; set; } = null!;
    }
}
