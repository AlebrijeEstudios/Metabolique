using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Feeding_Dtos
{
    public class FoodsConsumedDto
    {
        [JsonRequired] public string foodCode { get; set; } = null!;  

        [JsonRequired] public string nameFood { get; set; } = null!;

        [JsonRequired] public string unit { get; set; } = null!;

        [JsonRequired] public List<NutritionalValuesDto> nutritionalValues { get; set; } = null!;
    }
}
