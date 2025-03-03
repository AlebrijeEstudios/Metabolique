using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos
{
    public class TokenAdminDto
    {
        [JsonRequired] public Guid doctorID { get; set; }

        [JsonRequired] public string accessToken { get; set; } = null!;
    }
}
