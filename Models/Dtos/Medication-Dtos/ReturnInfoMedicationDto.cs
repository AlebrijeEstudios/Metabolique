using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Medication_Dtos
{
    public class ReturnInfoMedicationDto
    {
        [JsonRequired] public Guid medicationID { get; set; }

        [JsonRequired] public string nameMedication { get; set; } = null!;

        [JsonRequired] public int dose { get; set; }

        [JsonRequired] public string initialFrec { get; set; } = null!;

        [JsonRequired] public string finalFrec { get; set; } = null!;

        [JsonRequired] public int dailyFrec { get; set; }

        [JsonRequired] List<TimeListDto> times { get; set; } = null!;

    }
}
