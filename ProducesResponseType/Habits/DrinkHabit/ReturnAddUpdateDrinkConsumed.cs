using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;

namespace AppVidaSana.ProducesResponseType.Habits.DrinkHabit
{
    public class ReturnAddUpdateDrinkConsumed
    {
        public bool message { get; set; } = true;

        public GetDrinksConsumedDto drinksConsumed { get; set; } = null!;
    }
}
