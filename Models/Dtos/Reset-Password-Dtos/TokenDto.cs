using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Reset_Password_Dtos
{
    public class TokenDto
    {
        [JsonRequired] public string token { get; set; } = null!;

        [JsonRequired] public Guid accountID { get; set; }
    }
}
