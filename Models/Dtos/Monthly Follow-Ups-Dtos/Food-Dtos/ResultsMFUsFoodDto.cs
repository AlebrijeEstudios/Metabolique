using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Food_Dtos
{
    public class ResultsMFUsFoodDto
    {
        [JsonRequired] public Guid monthlyFollowUpID { get; set; } 
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
