using AppVidaSana.Models.Dtos.Habits_Dtos;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;

namespace AppVidaSana.ProducesResponseType.Habits
{
    public class ReturnHabitsInfo
    {
        public string message { get; set; } = "Ok.";

        public ReturnInfoHabitsDto habits { get; set; } = null!;
    }
}
