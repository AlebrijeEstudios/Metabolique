using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Medication_Dtos
{
    public class TimeListDto
    {
        [JsonRequired] public Guid timeID { get; set; }

        [JsonRequired] public Guid periodID { get; set; }

        [JsonRequired] public DateOnly dateMedication { get; set; }

        [JsonRequired] public TimeOnly time { get; set; }

        [JsonRequired] public bool medicationStatus { get; set; }
    }
}
