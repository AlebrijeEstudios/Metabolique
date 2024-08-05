using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos
{
    public class SaveResultsHabitsDto
    {
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public string month { get; set; } = null!;

        [JsonRequired] public int year { get; set; }

        [JsonRequired] public byte resultComponent1 { get; set; }

        [JsonRequired] public byte resultComponent2 { get; set; }

        [JsonRequired] public byte resultComponent3 { get; set; }

        [JsonRequired] public byte resultComponent4 { get; set; }

        [JsonRequired] public byte resultComponent5 { get; set; }

        [JsonRequired] public byte resultComponent6 { get; set; }

        [JsonRequired] public byte resultComponent7 { get; set; }

        [JsonRequired] public int globalClassification { get; set; }

        [JsonRequired] public string classification { get; set; } = null!;

    }
}
