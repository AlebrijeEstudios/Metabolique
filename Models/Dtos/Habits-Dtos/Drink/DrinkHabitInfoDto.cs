using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.Drink
{
    public class DrinkHabitInfoDto
    {
        [JsonRequired] public Guid drinkHabitID { get; set; } 

        public int? amountConsumed { get; set; }
    }
}
