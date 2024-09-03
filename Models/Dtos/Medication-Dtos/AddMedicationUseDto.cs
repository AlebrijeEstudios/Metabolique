using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Medication_Dtos
{
    public class AddMedicationUseDto
    {
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public DateOnly dateActual { get; set; }

        [JsonRequired] public string nameMedication { get; set; } = null!;

        [JsonRequired] public int dose { get; set; }

        [JsonRequired] public DateOnly initialFrec { get; set; }

        [JsonRequired] public DateOnly finalFrec { get; set; } 

        [JsonRequired] public string times { get; set; } = null!;
    }
}
