using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Medication_Dtos
{
    public class UpdateMedicationUseDto
    {
        [JsonRequired] public Guid medicationID { get; set; }

        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public DateOnly dateRecord { get; set; }

        [JsonRequired] public string nameMedication { get; set; } = null!;

        [JsonRequired] public int dose { get; set; }

        [JsonRequired] public DateOnly initialFrec { get; set; }

        [JsonRequired] public DateOnly finalFrec { get; set; }

        [JsonRequired] public List<TimeListDto> timesPrevious { get; set; } = null!;

        [JsonRequired] public List<TimeOnly> times { get; set; } = new List<TimeOnly>();
    }
}
