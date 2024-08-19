using AppVidaSana.Models.Dtos.Medication_Dtos;

namespace AppVidaSana.ProducesResponseType.Medications
{
    public class ReturnAddUpdateMedication
    {
        public bool message { get; set; } = true;

        public InfoMedicationDto medication { get; set; } = null!;
    }
}
