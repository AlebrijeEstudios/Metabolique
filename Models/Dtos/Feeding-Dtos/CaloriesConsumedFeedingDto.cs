using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Feeding_Dtos
{
    public class CaloriesConsumedFeedingDto 
    {
        [JsonRequired] public DateOnly date { get; set; } 

        [JsonRequired] public double limit { get; set; }

        [JsonRequired] public double value { get; set; }
    }
}
