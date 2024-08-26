using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Medication_Dtos
{
    public class WeeklyAttachmentDto
    {
        [JsonRequired] public DateOnly date { get; set; }

        [JsonRequired] public int totalMedications { get; set; }

        [JsonRequired] public int medicationsConsumed { get; set; }
    }
}
