using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos
{
    public class AllCaloriesConsumedPerUserDto
    {
        [JsonRequired] public Guid caloriesConsumedID { get; set; }

        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public string username { get; set; } = null!;

        [JsonRequired] public DateOnly date { get; set; }

        [JsonRequired] public double totalKcal { get; set; }

    }
}
