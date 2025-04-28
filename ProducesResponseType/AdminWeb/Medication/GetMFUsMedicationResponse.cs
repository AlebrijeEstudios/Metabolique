using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Medication
{
    public class GetMFUsMedicationResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllMFUsMedicationsPerUserDto> mfu { get; set; } = null!;
    }
}
