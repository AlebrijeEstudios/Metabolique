﻿using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Medication_Dtos
{
    public class InfoMedicationDto
    {
        [JsonRequired] public Guid medicationID { get; set; }

        [JsonRequired] public Guid periodID { get; set; }

        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public string nameMedication { get; set; } = null!;

        [JsonRequired] public string dose { get; set; } = null!;

        [JsonRequired] public DateOnly initialFrec { get; set; }

        [JsonRequired] public DateOnly finalFrec { get; set; }

        [JsonRequired] public List<TimeListDto> times { get; set; } = null!;

    }
}
