using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Feeding_Dtos
{
    public class InfoGeneralFeedingDto
    {
        public List<StatusDailyMealsDto>? defaultDailyMeals { get; set; }   
        
        public List<StatusDailyMealsDto>? othersDailyMeals { get; set; }  

        [JsonRequired] public List<CaloriesConsumedFeedingDto> caloriesConsumed { get; set; } = null!;

        [JsonRequired] public bool mfuStatus { get; set; }  
    }
}
