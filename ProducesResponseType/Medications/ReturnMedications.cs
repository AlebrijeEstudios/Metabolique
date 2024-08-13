using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Medication_Dtos;

namespace AppVidaSana.ProducesResponseType.Medications
{
    public class ReturnMedications
    {
        public bool message { get; set; } = true;

        public List<InfoMedicationDto> medications { get; set; } = null!;

        public List<WeeklyAttachmentDto> weeklyAttachments { get; set; } = null!;
    }
}
