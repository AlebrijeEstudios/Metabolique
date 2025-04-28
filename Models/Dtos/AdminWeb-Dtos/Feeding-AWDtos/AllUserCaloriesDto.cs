using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos
{
    public class AllUserCaloriesDto
    {
        [JsonRequired] public Guid userCaloriesID { get; set; }

        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public string username { get; set; } = null!;

        [JsonRequired] public double kcalNeeded { get; set; }
    }
}
