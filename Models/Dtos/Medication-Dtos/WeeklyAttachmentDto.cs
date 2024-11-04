using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Medication_Dtos
{
    public class WeeklyAttachmentDto
    {
        [JsonRequired] public DateOnly date { get; set; }

        [JsonRequired] public int limit { get; set; }

        [JsonRequired] public int value { get; set; }
    }
}
