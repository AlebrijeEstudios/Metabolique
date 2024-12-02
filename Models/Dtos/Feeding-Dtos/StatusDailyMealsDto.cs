using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Feeding_Dtos
{
    public class StatusDailyMealsDto
    {
        [JsonRequired] public Guid userFeedID { get; set; } 

        [JsonRequired] public string nameDailyMeal { get; set; } = null!;

        [JsonRequired] public bool dailyMealStatus { get; set; }

        [JsonRequired] public float totalCalories { get; set; }
    }
}
