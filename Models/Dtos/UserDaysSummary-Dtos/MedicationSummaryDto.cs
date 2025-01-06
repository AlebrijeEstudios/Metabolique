using Newtonsoft.Json;


namespace AppVidaSana.Models.Dtos.UserDaysSummary_Dtos
{
    public class MedicationSummaryDto
    {
        [JsonRequired] public int limit { get; set; } = 0;

        [JsonRequired] public int value { get; set; } = 0;
    }
}
