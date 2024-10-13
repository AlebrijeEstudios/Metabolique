using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.Sleep
{
    public class SleepHabitInfoDto
    {
        [JsonRequired] public Guid sleepHabitID { get; set; }

        [JsonRequired] public int? sleepHours { get; set; }

        [JsonRequired] public string? perceptionOfRelaxation { get; set; }  
    }
}
