using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Medications_Dtos
{
    public class SaveResponsesMedicationsDto
    {
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public int month { get; set; }

        [JsonRequired] public int year { get; set; }

        [JsonRequired] public bool answerQuestion1 { get; set; }

        [JsonRequired] public bool answerQuestion2 { get; set; }

        [JsonRequired] public bool answerQuestion3 { get; set; }

        [JsonRequired] public bool answerQuestion4 { get; set; }
    }
}
