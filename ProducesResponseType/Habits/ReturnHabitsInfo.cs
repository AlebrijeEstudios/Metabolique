using AppVidaSana.Models.Dtos.Habits_Dtos;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.Models.Dtos.Habits_Dtos.Sleep_And_Drugs;

namespace AppVidaSana.ProducesResponseType.Habits
{
    public class ReturnHabitsInfo
    {
        public bool message { get; set; } = true;

        public List<GetDrinksConsumedDto> drinkConsumed { get; set; } = null!;

        public GetSleepingHoursDto hoursSleepConsumed { get; set; } = null!;

        public GetDrugsConsumedDto drugsConsumed { get; set; } = null!;

        public List<GetSleepingHoursDto> hoursSleep { get; set; } = null!;
    }
}
