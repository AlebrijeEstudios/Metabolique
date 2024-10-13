using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.ReturnInfoHabits
{
    public class GetDrugsConsumedDto
    {
        [JsonRequired] public Guid drugsHabitID { get; set; }

        [JsonRequired] public DateOnly drugsDateHabit { get; set; }

        [JsonRequired] public int cigarettesSmoked { get; set; }

        [JsonRequired] public string predominantEmotionalState { get; set; } = null!;
    }
}
