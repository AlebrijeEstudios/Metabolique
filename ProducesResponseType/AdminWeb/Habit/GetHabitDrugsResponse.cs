using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Habits_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Habit
{
    public class GetHabitDrugsResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllHabitDrugPerUserDto> hDrugs { get; set; } = null!;
    }
}
