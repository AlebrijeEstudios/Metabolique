﻿using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Medication_Dtos
{
    public class AddMedicationUseDto
    {
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public DateOnly dateRecord { get; set; }

        [JsonRequired] public string nameMedication { get; set; } = null!;

        [JsonRequired] public int dose { get; set; }

        [JsonRequired] public DateOnly initialFrec { get; set; }

        [JsonRequired] public DateOnly finalFrec { get; set; } 

        [JsonRequired] public int dailyFrec { get; set; }

        [JsonRequired] public List<TimeOnly> times { get; set; } = null!;
    }
}