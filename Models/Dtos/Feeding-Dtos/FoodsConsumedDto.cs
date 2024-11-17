using Newtonsoft.Json;


namespace AppVidaSana.Models.Dtos.Feeding_Dtos
{
    public class FoodsConsumedDto
    {
        [JsonRequired] public string foodName { get; set; } = null!;

        [JsonRequired] public float portion { get; set; }

        [JsonRequired] public string unit { get; set; } = null!;

        [JsonRequired] public int netWeight { get; set; }

        [JsonRequired] public float kilocalories { get; set; }

        [JsonRequired] public float protein { get; set; }

        [JsonRequired] public float carbohydrates { get; set; }

        [JsonRequired] public float totalLipids { get; set; }
    }
}
