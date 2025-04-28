using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Medication
{
    public class GetPeriodMedResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllPeriodsMedicationsPerUserDto> periodsMed { get; set; } = null!;
    }
}
