using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Habits_AWDtos
{
    public class AllHabitDrinkPerUserDto
    {
        [JsonRequired] public Guid drinkHabitID { get; set; }

        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public string username { get; set; } = null!;

        [JsonRequired] public DateOnly dateHabit { get; set; } 

        [JsonRequired] public int amountConsumed { get; set; }
    }
}
