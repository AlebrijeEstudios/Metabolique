using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Feeding_Dtos
{
    public class InfoGeneralFeedingDto
    {
        [JsonRequired] public List<StatusDailyMealsDto> defaultDailyMeals { get; set; } = null!;  
        
        [JsonRequired] public List<StatusDailyMealsDto> othersDailyMeals { get; set; } = null!;  

        [JsonRequired] public List<CaloriesConsumedFeedingDto> caloriesConsumed { get; set; } = null!;

        [JsonRequired] public bool mfuStatus { get; set; }
    }
}
