using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Doctor_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Doctor
{
    public class GetDoctorsResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllDoctorsDto> doctors { get; set; } = null!;
    }
}
