using AppVidaSana.Models.Dtos.Medication_Dtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Medication
{
    public class GetInfoMedResponse
    {
        public string message { get; set; } = "Ok.";

        public List<InfoMedicationDto> meds { get; set; } = null!;
    }
}
