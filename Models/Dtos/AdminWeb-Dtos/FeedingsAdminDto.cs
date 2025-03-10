using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos
{
    public class FeedingsAdminDto
    {
        [JsonRequired] public Guid userFeedID { get; set; }

        [JsonRequired] public DateOnly userFeedDate { get; set; }

        [JsonRequired] public TimeOnly userFeedTime { get; set; } 

        [JsonRequired] public string dailyMeal { get; set; } = null!; 

        [JsonRequired] public string satietyLevel { get; set; } = null!;

        [JsonRequired] public string emotionsLinked { get; set; } = null!;

        [JsonRequired] public double totalCalories { get; set; } 

        public string? saucerPictureUrl { get; set; }
    }
}
