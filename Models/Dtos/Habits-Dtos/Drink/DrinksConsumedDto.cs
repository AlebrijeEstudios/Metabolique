using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.Drink
{
    public class DrinksConsumedDto
    {
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public DateOnly drinkDateHabit { get; set; }

        [JsonRequired] public string typeDrink { get; set; } = null!;

        [JsonRequired] public string amountConsumed { get; set; } = null!;

    }
}
