using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos
{
    public class ReturnSleepHoursAndDrugsConsumedDto
    {
        [JsonRequired] public DateOnly dateRegister { get; set; }

        [JsonRequired] public Guid sleepHabitID { get; set; }

        [JsonRequired] public int sleepHours { get; set; }

        [JsonRequired] public string perceptionOfRelaxation { get; set; } = null!;

        [JsonRequired] public Guid drugsHabitID { get; set; }

        [JsonRequired] public int cigarettesSmoked { get; set; }

        [JsonRequired] public string predominantEmotionalState { get; set; } = null!;
    }
}
