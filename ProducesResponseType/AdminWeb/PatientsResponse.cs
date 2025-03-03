using AppVidaSana.Models.Dtos.Account_Profile_Dtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb
{
    public class PatientsResponse
    {
        public string message { get; set; } = "Ok.";

        public List<InfoAccountDto> patients { get; set; } = null!;
    }
}
