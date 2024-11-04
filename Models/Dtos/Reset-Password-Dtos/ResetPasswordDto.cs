using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Reset_Password_Dtos
{
    public class ResetPasswordDto
    {
        [JsonRequired] public string email { get; set; } = null!;

        [JsonRequired] public string token { get; set; } = null!;

        [JsonRequired] public string password { get; set; } = null!;

        [JsonRequired] public string confirmPassword { get; set; } = null!;

    }
}
