using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Feeding_Dtos
{
    public class UpdateFeedingDto
    {
        [JsonRequired] public Guid userFeedID { get; set; }

        [JsonRequired] public DateOnly userFeedDate { get; set; }

        [JsonRequired] public TimeOnly userFeedTime { get; set; }

        [JsonRequired] public string dailyMeal { get; set; } = null!;

        [JsonRequired] public string foodsConsumed { get; set; } = null!;

        [JsonRequired] public string satietyLevel { get; set; } = null!;

        [JsonRequired] public string emotionsLinked { get; set; } = null!;

        public IFormFile? saucerPicture { get; set; }
    }
}
