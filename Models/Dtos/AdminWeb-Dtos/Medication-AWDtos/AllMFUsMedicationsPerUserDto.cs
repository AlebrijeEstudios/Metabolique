using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos
{
    public class AllMFUsMedicationsPerUserDto
    {
        [JsonRequired] public Guid monthlyFollowUpID { get; set; }
        [JsonRequired] public Guid accountID { get; set; }
        [JsonRequired] public string username { get; set; } = null!;

        [JsonRequired] public string month { get; set; } = null!;
        [JsonRequired] public int year { get; set; }

        [JsonRequired] public bool answerQuestion1 { get; set; }
        [JsonRequired] public bool answerQuestion2 { get; set; }
        [JsonRequired] public bool answerQuestion3 { get; set; }
        [JsonRequired] public bool answerQuestion4 { get; set; }

        [JsonRequired] public string statusAdherence { get; set; } = null!;
    }
}
