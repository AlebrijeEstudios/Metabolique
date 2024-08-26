using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.Sleep_And_Drugs
{
    public class GetHoursSleepConsumedDto
    {
        [JsonRequired] public Guid sleepHabitID { get; set; }

        [JsonRequired] public DateOnly sleepDateHabit { get; set; }

        [JsonRequired] public int sleepHours { get; set; }

        [JsonRequired] public string perceptionOfRelaxation { get; set; } = null!;
    }
}
