using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.Drugs
{
    public class DrugsHabitDto
    { 
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public DateOnly dateRegister { get; set; }

        [JsonRequired] public int? cigarettesSmoked { get; set; }

        [JsonRequired] public string? predominantEmotionalState { get; set; }
    }
}
