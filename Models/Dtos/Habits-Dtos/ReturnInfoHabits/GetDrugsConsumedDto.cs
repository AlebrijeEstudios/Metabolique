using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.ReturnInfoHabits
{
    public class GetDrugsConsumedDto
    {
        [JsonRequired] public Guid drugsHabitID { get; set; }

        [JsonRequired] public DateOnly drugsDateHabit { get; set; }

        public int? cigarettesSmoked { get; set; }

        public string? predominantEmotionalState { get; set; } 
    }
}
