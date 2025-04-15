using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos
{
    public class AllFeedsOfAUserDto
    {
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public Guid userFeedID { get; set; }

        [JsonRequired] public string userName { get; set; } = null!;

        [JsonRequired] public DateOnly userFeedDate { get; set; }

        [JsonRequired] public TimeOnly userFeedTime { get; set; }

        [JsonRequired] public string dailyMeal { get; set; } = null!;

        [JsonRequired] public double totalCarbohydrates { get; set; }

        [JsonRequired] public double totalProtein { get; set; }

        [JsonRequired] public double totalLipids { get; set; }

        [JsonRequired] public double totalCalories { get; set; }

        [JsonRequired] public double totalNetWeight { get; set; }

        [JsonRequired] public string satietyLevel { get; set; } = null!;

        [JsonRequired] public string emotionsLinked { get; set; } = null!;

        public string? saucerPictureUrl { get; set; }
    }
}
