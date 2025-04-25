using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb
{
    public class GetPatientsResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllPatientsDto> patients { get; set; } = null!;
    }
}
