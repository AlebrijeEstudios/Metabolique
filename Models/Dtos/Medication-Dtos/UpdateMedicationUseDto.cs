using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Medication_Dtos
{
    public class UpdateMedicationUseDto
    {
        [JsonRequired] public Guid periodID { get; set; }

        [JsonRequired] public DateOnly updateDate { get; set; }

        [JsonRequired] public string nameMedication { get; set; } = null!;

        [JsonRequired] public int dose { get; set; }
        
        [JsonRequired] public DateOnly initialFrec { get; set; }

        [JsonRequired] public DateOnly finalFrec { get; set; }

        [JsonRequired] public List<TimeListDto> times { get; set; } = null!;

        [JsonRequired] public string newTimes { get; set; } = null!;

    }
}
