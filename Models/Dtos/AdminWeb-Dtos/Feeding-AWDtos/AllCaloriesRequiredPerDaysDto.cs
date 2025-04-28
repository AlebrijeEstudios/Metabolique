using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos
{
    public class AllCaloriesRequiredPerDaysDto
    {
        [JsonRequired] public Guid caloriesPerDayID { get; set; }

        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public string username { get; set; } = null!;

        [JsonRequired] public DateOnly dateInitial { get; set; }

        [JsonRequired] public DateOnly dateFinal { get; set; }

        [JsonRequired] public double kcalNeeded { get; set; }
    }
}
