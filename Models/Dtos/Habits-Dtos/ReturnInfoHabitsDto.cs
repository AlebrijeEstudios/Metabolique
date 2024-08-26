using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.Models.Dtos.Habits_Dtos.Sleep_And_Drugs;
using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos
{
    public class ReturnInfoHabitsDto
    {
        public List<GetDrinksConsumedDto> drinkConsumed { get; set; } = null!;

        public GetHoursSleepConsumedDto hoursSleepConsumed { get; set; } = null!;

        public GetDrugsConsumedDto drugsConsumed { get; set; } = null!;
         
        public List<GraphicValuesHabitSleepDto> hoursSleep { get; set; } = null!;

    }
}
