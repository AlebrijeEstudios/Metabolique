using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Doctor_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Doctor
{
    public class AddUpdateDoctorsResponse
    {
        public string message { get; set; } = "Ok.";

        public AllDoctorsDto doctor { get; set; } = null!;
    }
}
