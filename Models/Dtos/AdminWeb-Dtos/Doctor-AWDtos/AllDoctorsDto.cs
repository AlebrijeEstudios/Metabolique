using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Doctor_AWDtos
{
    public class AllDoctorsDto
    {
        [JsonRequired] public Guid doctorID { get; set; }

        [JsonRequired] public string username { get; set; } = null!;

        [JsonRequired] public string email { get; set; } = null!;

        [JsonRequired] public string role { get; set; } = null!;
    }
}
