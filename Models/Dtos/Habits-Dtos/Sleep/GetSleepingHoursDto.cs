using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos
{
    public class GetSleepingHoursDto
    {
        [JsonRequired] public Guid sleepHabitID { get; set; }

        [JsonRequired] public DateOnly sleepDateHabit { get; set; }

        [JsonRequired] public int sleepHours { get; set; }

        [JsonRequired] public string perceptionOfRelaxation { get; set; } = null!;
    }
}
