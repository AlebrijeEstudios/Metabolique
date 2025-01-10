using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.ReturnInfoHabits
{
    public class GetDrinkConsumedDto
    {
        [JsonRequired] public Guid drinkHabitID { get; set; }

        [JsonRequired] public DateOnly drinkDateHabit { get; set; }

        public int? amountConsumed { get; set; }
    }
}
