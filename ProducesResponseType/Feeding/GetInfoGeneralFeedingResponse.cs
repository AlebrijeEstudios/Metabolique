using AppVidaSana.Models.Dtos.Feeding_Dtos;

namespace AppVidaSana.ProducesResponseType.Feeding
{
    public class GetInfoGeneralFeedingResponse
    {
        public string message { get; set; } = "Ok.";

        public List<StatusDailyMealsDto>? defaultDailyMeals { get; set; }

        public List<StatusDailyMealsDto>? othersDailyMeals { get; set; }

        public List<CaloriesConsumedFeedingDto> caloriesConsumed { get; set; } = null!;

        public bool mfuStatus { get; set; }
    }
}
