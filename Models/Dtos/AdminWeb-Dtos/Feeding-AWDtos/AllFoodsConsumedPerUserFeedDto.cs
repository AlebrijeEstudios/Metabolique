using AppVidaSana.Models.Dtos.Feeding_Dtos;
using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos
{
    public class AllFoodsConsumedPerUserFeedDto
    {
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public Guid userFeedID { get; set; }

        [JsonRequired] public DateOnly userFeedDate { get; set; }

        [JsonRequired] public TimeOnly userFeedTime { get; set; }

        [JsonRequired] public string dailyMeal { get; set; } = null!;

        [JsonRequired] public List<FoodsConsumedDto> foodsConsumed { get; set; } = null!;
    }
}
