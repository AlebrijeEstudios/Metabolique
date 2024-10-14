using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.Sleep
{
    public class SleepHabitInfoDto
    {
        [JsonRequired] public Guid sleepHabitID { get; set; }

        public int? sleepHours { get; set; }

        public string? perceptionOfRelaxation { get; set; }  
    }
}
