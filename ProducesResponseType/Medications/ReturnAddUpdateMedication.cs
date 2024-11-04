using AppVidaSana.Models.Dtos.Medication_Dtos;

namespace AppVidaSana.ProducesResponseType.Medications
{
    public class ReturnAddUpdateMedication
    {
        public string message { get; set; } = "Ok.";

        public InfoMedicationDto? medication { get; set; } = null!;
    }
}
