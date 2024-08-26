using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Exercise_Dtos
{
    public class UpdateResponsesExerciseDto
    {
        [JsonRequired] public Guid monthlyFollowUpID { get; set; }

        [JsonRequired] public int month { get; set; }

        [JsonRequired] public int year { get; set; }

        [JsonRequired] public int question1 { get; set; }

        [JsonRequired] public int question2 { get; set; }

        [JsonRequired] public int question3 { get; set; }

        [JsonRequired] public int question4 { get; set; }

        [JsonRequired] public int question5 { get; set; }

        [JsonRequired] public int question6 { get; set; }

        [JsonRequired] public int question7 { get; set; }

    }
}
