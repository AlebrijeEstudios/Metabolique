using AppVidaSana.Models.Dtos.Doctor_Dtos;

namespace AppVidaSana.ProducesResponseType
{
    public class DoctorResponse
    {
        public string message { get; set; } = "Ok.";

        public List<DoctorDto> doctors { get; set; } = null!;
    }
}
