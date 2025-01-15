using AppVidaSana.Models.Dtos.Habits_Dtos.ReturnInfoHabits;

namespace AppVidaSana.ProducesResponseType.Habits
{
    public class ReturnHabitsInfo
    {
        public string message { get; set; } = "Ok.";

        public GetDrinkConsumedDto drinkConsumed { get; set; } = null!;

        public GetHoursSleepConsumedDto hoursSleepConsumed { get; set; } = null!;

        public GetDrugsConsumedDto drugsConsumed { get; set; } = null!;

        public List<GraphicValuesHabitSleepDto> hoursSleep { get; set; } = null!;

        public bool mfuStatus { get; set; }
    }
}
