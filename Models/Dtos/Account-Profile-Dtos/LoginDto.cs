using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Account_Profile_Dtos
{
    public class LoginDto
    {
        [JsonRequired] public string email { get; set; } = null!;

        [JsonRequired] public string password { get; set; } = null!;

    }
}
