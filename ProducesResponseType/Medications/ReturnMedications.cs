using AppVidaSana.Models.Dtos.Medication_Dtos;

namespace AppVidaSana.ProducesResponseType.Medications
{
    public class ReturnMedications
    {
        public string message { get; set; } = "Ok.";

        public List<InfoMedicationDto> medications { get; set; } = null!;

        public List<WeeklyAttachmentDto> weeklyAttachments { get; set; } = null!;

        public List<SideEffectsListDto> sideEffects { get; set; } = null!;
    }
}
