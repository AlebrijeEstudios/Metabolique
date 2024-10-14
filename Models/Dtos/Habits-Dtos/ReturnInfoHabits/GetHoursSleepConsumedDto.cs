using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.ReturnInfoHabits
{
    public class GetHoursSleepConsumedDto
    {
        [JsonRequired] public Guid sleepHabitID { get; set; }

        [JsonRequired] public DateOnly sleepDateHabit { get; set; }

        public int? sleepHours { get; set; }

        public string? perceptionOfRelaxation { get; set; } = null!;
    }
}
