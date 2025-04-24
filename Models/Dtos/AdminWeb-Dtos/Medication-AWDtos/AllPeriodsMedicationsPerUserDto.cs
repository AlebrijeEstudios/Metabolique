using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos
{
    public class AllPeriodsMedicationsPerUserDto
    {
        [JsonRequired] public Guid periodID { get; set; }

        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public string username { get; set; } = null!;

        [JsonRequired] public string medication { get; set; } = null!;

        [JsonRequired] public DateOnly initialDate { get; set; }

        [JsonRequired] public DateOnly finalDate { get; set; }

        [JsonRequired] public string daysEliminated { get; set; } = null!;

        [JsonRequired] public string dose { get; set; } = null!;

        [JsonRequired] public DateOnly dateConsumed { get; set; }

        [JsonRequired] public TimeOnly timeConsumed { get; set; }

        [JsonRequired] public bool statusConsumed { get; set; } 
    }
}
