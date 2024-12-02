using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Token_RefreshToken_Dtos
{
    public class RefreshTokenDto
    {

        [JsonRequired] public string tokenExpired { get; set; } = null!;

        [JsonRequired] public string refreshToken { get; set; } = null!;

    }
}
