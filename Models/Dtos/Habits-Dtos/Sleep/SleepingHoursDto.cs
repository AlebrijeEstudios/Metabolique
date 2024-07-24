using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos
{
    public class SleepingHoursDto
    {
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public DateOnly sleepDateHabit { get; set; }

        [JsonRequired] public int sleepHours { get; set; }

        [JsonRequired] public string perceptionOfRelaxation { get; set; } = null!;

    }
}
