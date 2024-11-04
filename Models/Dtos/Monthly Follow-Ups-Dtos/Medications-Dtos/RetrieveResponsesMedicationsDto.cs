using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Medications_Dtos
{
    public class RetrieveResponsesMedicationsDto
    {
        [JsonRequired] public Guid monthlyFollowUpID { get; set; }

        [JsonRequired] public string month { get; set; } = null!;

        [JsonRequired] public int year { get; set; }

        [JsonRequired] public bool answerQuestion1 { get; set; }

        [JsonRequired] public bool answerQuestion2 { get; set; }

        [JsonRequired] public bool answerQuestion3 { get; set; }

        [JsonRequired] public bool answerQuestion4 { get; set; }

        [JsonRequired] public string statusAdherence { get; set; } = null!;

    }
}
