using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.Sleep_And_Drugs
{
    public class GraphicValuesHabitSleepDto
    {
        [JsonRequired] public DateOnly date { get; set; }

        [JsonRequired] public int value { get; set; }

    }
}
