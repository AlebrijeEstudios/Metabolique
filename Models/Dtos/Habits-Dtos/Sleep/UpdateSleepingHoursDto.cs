using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos
{
    public class UpdateSleepingHoursDto
    {
        [JsonRequired] public Guid sleepHabitID { get; set; }

        [JsonRequired] public int sleepHours { get; set; }

        [JsonRequired] public string perceptionOfRelaxation { get; set; } = null!;
    }
}
