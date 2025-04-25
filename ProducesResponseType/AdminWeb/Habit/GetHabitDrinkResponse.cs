using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Habits_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Habit
{
    public class GetHabitDrinkResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllHabitDrinkPerUserDto> hDrink { get; set; } = null!;
    }
}
