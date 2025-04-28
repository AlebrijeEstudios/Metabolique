using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Habits_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Habit
{
    public class GetMFUsHabitsResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllMFUsHabitsPerUserDto> mfu { get; set; } = null!;
    }
}
