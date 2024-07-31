using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Medication_Dtos
{
    public class TimeListDto
    {
        [JsonRequired] public Guid timeID { get; set; } = Guid.NewGuid();

        [JsonRequired] public Guid medicationID { get; set; }

        [JsonRequired] public TimeOnly time { get; set; }

        [JsonRequired] public bool medicationStatus { get; set; }
    }
}
