using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Reset_Password_Dtos
{
    public class TokensDto
    {
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public string accessToken { get; set; } = null!;

        [JsonRequired] public string refreshToken { get; set; } = null!;

    }
}
