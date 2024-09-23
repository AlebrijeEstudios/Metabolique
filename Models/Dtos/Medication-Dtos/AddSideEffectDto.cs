using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Medication_Dtos
{
    public class AddSideEffectDto
    {

        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public DateOnly date { get; set; }

        [JsonRequired] public TimeOnly initialTime { get; set; }

        [JsonRequired] public TimeOnly finalTime { get; set; }

        [JsonRequired] public string description { get; set; } = null!;

    }
}
