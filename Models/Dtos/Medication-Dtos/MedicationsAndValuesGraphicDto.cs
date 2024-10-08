﻿using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Medication_Dtos
{
    public class MedicationsAndValuesGraphicDto
    {
        [JsonRequired] public List<InfoMedicationDto> medications { get; set; } = null!;

        [JsonRequired] public List<WeeklyAttachmentDto> weeklyAttachments { get; set; } = null!;

        [JsonRequired] public List<SideEffectsListDto> sideEffects { get; set; } = null!;

        [JsonRequired] public bool mfuStatus { get; set; }
    }
}
