using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos
{
    public class AllMFUsFeedingPerUserDto
    {
        [JsonRequired] public Guid monthlyFollowUpID { get; set; }
        [JsonRequired] public Guid accountID { get; set; }
        [JsonRequired] public string username { get; set; } = null!;

        [JsonRequired] public string month { get; set; } = null!;
        [JsonRequired] public int year { get; set; }

        [JsonRequired] public float answerQuestion1 { get; set; }
        [JsonRequired] public float answerQuestion2 { get; set; }
        [JsonRequired] public float answerQuestion3 { get; set; }
        [JsonRequired] public float answerQuestion4 { get; set; }
        [JsonRequired] public float answerQuestion5 { get; set; }
        [JsonRequired] public float answerQuestion6 { get; set; }
        [JsonRequired] public float answerQuestion7 { get; set; }
        [JsonRequired] public float answerQuestion8 { get; set; }
        [JsonRequired] public float answerQuestion9 { get; set; }

        [JsonRequired] public float totalPts { get; set; }
        [JsonRequired] public string classification { get; set; } = null!;
    }
}
