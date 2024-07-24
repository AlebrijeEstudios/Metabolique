using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.Drugs
{
    public class GetDrugsConsumedDto
    {
        [JsonRequired] public Guid drugsHabitID { get; set; } = Guid.NewGuid();

        [JsonRequired] public int cigarettesSmoked { get; set; }

        [JsonRequired] public string predominantEmotionalState { get; set; } = null!;

    }
}
