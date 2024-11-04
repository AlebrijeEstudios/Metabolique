using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Reset_Password_Dtos
{
    public class EmailDto
    {
        [JsonRequired] public string email { get; set; } = null!;
    }
}
