using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos
{
    public class SaveResponsesHabitsDto
    {
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public string month { get; set; } = null!;

        [JsonRequired] public int year { get; set; }

        [JsonRequired] public TimeOnly answerQuestion1 { get; set; }

        [JsonRequired] public int answerQuestion2 { get; set; }

        [JsonRequired] public TimeOnly answerQuestion3 { get; set; }

        [JsonRequired] public int answerQuestion4 { get; set; }

        [JsonRequired] public int answerQuestion5a { get; set; }

        [JsonRequired] public int answerQuestion5b { get; set; }

        [JsonRequired] public int answerQuestion5c { get; set; }

        [JsonRequired] public int answerQuestion5d { get; set; }

        [JsonRequired] public int answerQuestion5e { get; set; }

        [JsonRequired] public int answerQuestion5f { get; set; }

        [JsonRequired] public int answerQuestion5g { get; set; }

        [JsonRequired] public int answerQuestion5h { get; set; }

        [JsonRequired] public int answerQuestion5i { get; set; }

        [JsonRequired] public int answerQuestion5j { get; set; }

        [JsonRequired] public int answerQuestion6 { get; set; }

        [JsonRequired] public int answerQuestion7 { get; set; }

        [JsonRequired] public int answerQuestion8 { get; set; }

        [JsonRequired] public int answerQuestion9 { get; set; }
    }
}
