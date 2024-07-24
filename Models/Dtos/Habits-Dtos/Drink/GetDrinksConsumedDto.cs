using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.Drink
{
    public class GetDrinksConsumedDto
    {
        [JsonRequired] public Guid drinkHabitID { get; set; }

        [JsonRequired] public string typeDrink { get; set; } = null!;

        [JsonRequired] public int amountConsumed { get; set; }

    }
}
