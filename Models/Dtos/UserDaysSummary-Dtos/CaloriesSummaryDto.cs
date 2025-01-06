using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.UserDaysSummary_Dtos
{
    public class CaloriesSummaryDto
    {
        [JsonRequired] public double limit { get; set; } = 0;

        [JsonRequired] public double value { get; set; } = 0;
    }
}
