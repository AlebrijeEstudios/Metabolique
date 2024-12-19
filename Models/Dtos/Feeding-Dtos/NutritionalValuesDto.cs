using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Feeding_Dtos
{
    public class NutritionalValuesDto
    {
        [JsonRequired] public string nutritionalValueCode { get; set; } = null!;

        [JsonRequired] public string portion { get; set; } = null!;

        [JsonRequired] public float kilocalories { get; set; }

        [JsonRequired] public float protein { get; set; }

        [JsonRequired] public float carbohydrates { get; set; }

        [JsonRequired] public float totalLipids { get; set; }
    }
}
