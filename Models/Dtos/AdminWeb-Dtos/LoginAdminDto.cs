using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos
{
    public class LoginAdminDto
    {
        [JsonRequired] public string username { get; set; } = null!;

        [JsonRequired] public string password { get; set; } = null!;
    }
}
