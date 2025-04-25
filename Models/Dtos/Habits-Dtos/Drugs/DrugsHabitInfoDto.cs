using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.Drugs
{
    public class DrugsHabitInfoDto
    {
        [JsonRequired] public Guid drugsHabitID { get; set; }

        public int? cigarettesSmoked { get; set; }

        public string? predominantEmotionalState { get; set; }
    }
}
