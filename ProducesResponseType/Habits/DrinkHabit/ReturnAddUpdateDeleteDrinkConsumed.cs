using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;

namespace AppVidaSana.ProducesResponseType.Habits.DrinkHabit
{
    public class ReturnAddUpdateDeleteDrinkConsumed
    {
        public string message { get; set; } = "Ok.";

        public List<GetDrinksConsumedDto> drinksConsumed { get; set; } = null!;
    }
}
