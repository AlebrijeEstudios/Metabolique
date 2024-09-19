using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Medication_Dtos
{
    public class SideEffectsListDto
    {
        [JsonRequired] public Guid sideEffectID { get; set; }

        [JsonRequired] public DateOnly date { get; set; }

        [JsonRequired] public TimeOnly initialTime { get; set; }

        [JsonRequired] public TimeOnly finalTime { get; set; }

        [JsonRequired] public string description { get; set; } = null!;
    }
}
