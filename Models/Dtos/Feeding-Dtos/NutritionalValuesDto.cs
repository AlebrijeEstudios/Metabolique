using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Feeding_Dtos
{
    public class NutritionalValuesDto
    {
        [JsonRequired] public string nutritionalValueCode { get; set; } = null!;

        [JsonRequired] public string portion { get; set; } = null!;

        [JsonRequired] public double kilocalories { get; set; }

        [JsonRequired] public double protein { get; set; }

        [JsonRequired] public double carbohydrates { get; set; }

        [JsonRequired] public double totalLipids { get; set; }
    }
}
