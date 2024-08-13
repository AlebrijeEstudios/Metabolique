using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Medication_Dtos
{
    public class MedicationDigestDto
    {
        [JsonRequired] public Guid medicationID { get; set; }

        [JsonRequired] public bool statusGeneral { get; set; }

    }
}
