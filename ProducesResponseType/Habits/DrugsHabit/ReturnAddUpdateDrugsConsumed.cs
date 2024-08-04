using AppVidaSana.Models.Dtos.Habits_Dtos.Drugs;

namespace AppVidaSana.ProducesResponseType.Habits.DrugsHabit
{
    public class ReturnAddUpdateDrugsConsumed
    {
        public string message { get; set; } = "Ok.";

        public GetDrugsConsumedDto drugsConsumed { get; set; } = null!;
    }
}
