using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;

namespace AppVidaSana.ProducesResponseType.Habits.DrinkHabit
{
    public class ReturnAddUpdateDrinkConsumed
    {
        public string message { get; set; } = "Ok.";

        public GetDrinksConsumedDto drinksConsumed { get; set; } = null!;
    }
}
