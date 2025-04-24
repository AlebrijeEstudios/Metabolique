using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos
{
    public class AllSideEffectsPerUserDto
    {
        [JsonRequired] public Guid sideEffectID { get; set; }

        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public string username { get; set; } = null!;

        [JsonRequired] public DateOnly date { get; set; } 

        [JsonRequired] public TimeOnly initialTime { get; set; }

        [JsonRequired] public TimeOnly finalTime { get; set; }

        [JsonRequired] public string description { get; set; } = null!;
    }
}
