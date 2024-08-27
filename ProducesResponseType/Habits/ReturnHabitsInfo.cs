using AppVidaSana.Models.Dtos.Habits_Dtos;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.Models.Dtos.Habits_Dtos.Sleep_And_Drugs;

namespace AppVidaSana.ProducesResponseType.Habits
{
    public class ReturnHabitsInfo
    {
        public string message { get; set; } = "Ok.";

        public List<GetDrinksConsumedDto> drinkConsumed { get; set; } = null!;

        public GetHoursSleepConsumedDto hoursSleepConsumed { get; set; } = null!;

        public GetDrugsConsumedDto drugsConsumed { get; set; } = null!;

        public List<GraphicValuesHabitSleepDto> hoursSleep { get; set; } = null!;
    }
}
