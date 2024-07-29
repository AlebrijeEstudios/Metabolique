using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drugs;

namespace AppVidaSana.ProducesResponseType.Habits
{
    public class ReturnGetDrugsConsumed
    {
        public string message { get; set; } = "Ok.";

        public GetDrugsConsumedDto drugsConsumed { get; set; } = null!;
    }
}
