using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.Models.Dtos.Habits_Dtos.ReturnInfoHabits;

namespace AppVidaSana.Models.Dtos.Habits_Dtos
{
    public class ReturnInfoHabitsDto
    {
        public GetDrinkConsumedDto drinkConsumed { get; set; } = null!;

        public GetHoursSleepConsumedDto hoursSleepConsumed { get; set; } = null!;

        public GetDrugsConsumedDto drugsConsumed { get; set; } = null!;

        public List<GraphicValuesHabitSleepDto> hoursSleep { get; set; } = null!;

        public bool mfuStatus { get; set; }

    }
}
