using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos
{
    public class RetrieveResponsesHabitsDto
    {
        [JsonRequired] public Guid monthlyFollowUpID { get; set; }

        [JsonRequired] public string month { get; set; } = null!;

        [JsonRequired] public int year { get; set; }

        [JsonRequired] public TimeOnly answerQuestion1 { get; set; }

        [JsonRequired] public byte answerQuestion2 { get; set; }

        [JsonRequired] public TimeOnly answerQuestion3 { get; set; }

        [JsonRequired] public int answerQuestion4 { get; set; }

        [JsonRequired] public byte answerQuestion5a { get; set; }

        [JsonRequired] public byte answerQuestion5b { get; set; }

        [JsonRequired] public byte answerQuestion5c { get; set; }

        [JsonRequired] public byte answerQuestion5d { get; set; }

        [JsonRequired] public byte answerQuestion5e { get; set; }

        [JsonRequired] public byte answerQuestion5f { get; set; }

        [JsonRequired] public byte answerQuestion5g { get; set; }

        [JsonRequired] public byte answerQuestion5h { get; set; }

        [JsonRequired] public byte answerQuestion5i { get; set; }

        [JsonRequired] public byte answerQuestion5j { get; set; }

        [JsonRequired] public byte answerQuestion6 { get; set; }

        [JsonRequired] public byte answerQuestion7 { get; set; }

        [JsonRequired] public byte answerQuestion8 { get; set; }

        [JsonRequired] public byte answerQuestion9 { get; set; }

        [JsonRequired] public byte resultComponent1 { get; set; }

        [JsonRequired] public byte resultComponent2 { get; set; }

        [JsonRequired] public byte resultComponent3 { get; set; }

        [JsonRequired] public byte resultComponent4 { get; set; }

        [JsonRequired] public byte resultComponent5 { get; set; }

        [JsonRequired] public byte resultComponent6 { get; set; }

        [JsonRequired] public byte resultComponent7 { get; set; }

        [JsonRequired] public int globalClassification { get; set; }

        [JsonRequired] public string classification { get; set; } = null!;
    }
}
