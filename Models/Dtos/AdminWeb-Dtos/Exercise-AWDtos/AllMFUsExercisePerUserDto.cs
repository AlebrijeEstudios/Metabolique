using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Exercise_AWDtos
{
    public class AllMFUsExercisePerUserDto
    {
        [JsonRequired] public Guid monthlyFollowUpID { get; set; }
        [JsonRequired] public Guid accountID { get; set; }
        [JsonRequired] public string username { get; set; } = null!;


        [JsonRequired] public string month { get; set; } = null!;
        [JsonRequired] public int year { get; set; }


        [JsonRequired] public int answerQuestion1 { get; set; }
        [JsonRequired] public int answerQuestion2 { get; set; }
        [JsonRequired] public int answerQuestion3 { get; set; }
        [JsonRequired] public int answerQuestion4 { get; set; }
        [JsonRequired] public int answerQuestion5 { get; set; }
        [JsonRequired] public int answerQuestion6 { get; set; }
        [JsonRequired] public int answerQuestion7 { get; set; }


        [JsonRequired] public float actWalking { get; set; }
        [JsonRequired] public float actModerate { get; set; }
        [JsonRequired] public float actVigorous { get; set; }
        [JsonRequired] public float totalMET { get; set; }
        [JsonRequired] public string sedentaryBehavior { get; set; } = null!;
        [JsonRequired] public string levelAF { get; set; } = null!;
    }
}
