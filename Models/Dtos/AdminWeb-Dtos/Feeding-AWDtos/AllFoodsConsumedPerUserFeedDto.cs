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


        [JsonRequired] public string foodCode { get; set; } = null!;

        [JsonRequired] public string nameFood { get; set; } = null!;

        [JsonRequired] public string unit { get; set; } = null!;

        [JsonRequired] public string nutritionalValueCode { get; set; } = null!;

        [JsonRequired] public string portion { get; set; } = null!;

        [JsonRequired] public double kilocalories { get; set; }

        [JsonRequired] public double protein { get; set; }

        [JsonRequired] public double carbohydrates { get; set; }

        [JsonRequired] public double totalLipids { get; set; }

        [JsonRequired] public float netWeight { get; set; }
    }
}
