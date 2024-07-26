using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos
{
    public class SaveResultsDto
    {
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public string month { get; set; } = null!;

        [JsonRequired] public int year { get; set; }

        [JsonRequired] public int resultComponent1 { get; set; }

        [JsonRequired] public int resultComponent2 { get; set; }

        [JsonRequired] public int resultComponent3 { get; set; }

        [JsonRequired] public int resultComponent4 { get; set; }

        [JsonRequired] public int resultComponent5 { get; set; }

        [JsonRequired] public int resultComponent6 { get; set; }

        [JsonRequired] public int resultComponent7 { get; set; }

        [JsonRequired] public int globalClassification { get; set; }

        [JsonRequired] public string classification { get; set; } = null!;

    }
}
