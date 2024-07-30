using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;

namespace AppVidaSana.ProducesResponseType.Habits
{
    public class ReturnAddUpdateDeleteDrinkConsumed
    {
        public string message { get; set; } = "Ok.";

        public List<GetDrinksConsumedDto> drinksConsumed { get; set; } = null!;
    }
}
