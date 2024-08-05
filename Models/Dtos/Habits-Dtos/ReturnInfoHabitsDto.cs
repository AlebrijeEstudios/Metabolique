using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drugs;
using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos
{
    public class ReturnInfoHabitsDto
    {
        public List<GetDrinksConsumedDto> drinkConsumed { get; set; } = null!;

        public GetSleepingHoursDto hoursSleep { get; set; } = null!;

        public GetDrugsConsumedDto drugsConsumed { get; set; } = null!;

    }
}
