using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.Sleep_And_Drugs
{
    public class SleepingHoursAndDrugsConsumedDto
    {
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public DateOnly dateRegister { get; set; }

        [JsonRequired] public int sleepHours { get; set; }

        [JsonRequired] public string perceptionOfRelaxation { get; set; } = null!;

        [JsonRequired] public int cigarettesSmoked { get; set; }

        [JsonRequired] public string predominantEmotionalState { get; set; } = null!;
    }
}
