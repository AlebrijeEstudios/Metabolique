using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Habits_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Habit
{
    public class GetHabitSleepResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllHabitSleepPerUserDto> hSleep { get; set; } = null!;
    }
}
