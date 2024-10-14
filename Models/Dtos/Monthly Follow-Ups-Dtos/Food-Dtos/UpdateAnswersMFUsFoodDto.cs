using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Food_Dtos
{
    public class UpdateAnswersMFUsFoodDto
    {
        [JsonRequired] public Guid monthlyFollowUpID { get; set; }
        [JsonRequired] public int month { get; set; }
        [JsonRequired] public int year { get; set; }

        [JsonRequired] public float question1 { get; set; }
        [JsonRequired] public float question2 { get; set; }
        [JsonRequired] public float question3 { get; set; }
        [JsonRequired] public float question4 { get; set; }
        [JsonRequired] public float question5 { get; set; }
        [JsonRequired] public float question6 { get; set; }
        [JsonRequired] public float question7 { get; set; }
        [JsonRequired] public float question8 { get; set; }
        [JsonRequired] public float question9 { get; set; }
    }
}
