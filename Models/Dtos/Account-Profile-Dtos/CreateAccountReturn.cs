using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Account_Profile_Dtos
{
    public class CreateAccountReturn
    {
        [JsonRequired] public string messageException { get; set; } = null!;

        [JsonRequired] public Guid accountID { get; set; }

    }
}
