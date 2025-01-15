using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;

namespace AppVidaSana.ProducesResponseType.Habits.DrinkHabit
{
    public class ResponseDrinkHabit
    {
        public string message { get; set; } = "Ok.";

        public DrinkHabitInfoDto drinkConsumed { get; set; } = null!;
    }
}
