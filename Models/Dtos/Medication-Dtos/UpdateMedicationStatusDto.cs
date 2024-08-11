using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Medication_Dtos
{
    public class UpdateMedicationStatusDto
    {
        [JsonRequired] public Guid timeID { get; set; }

        [JsonRequired] public bool medicationStatus { get; set; }
    }
}
