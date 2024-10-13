using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.Drugs
{
    public class DrugsHabitInfoDto
    {
        [JsonRequired] public Guid drugsHabitID { get; set; }

        [JsonRequired] public int cigarettesSmoked { get; set; }

        [JsonRequired] public string predominantEmotionalState { get; set; } = null!; 
    }
}
