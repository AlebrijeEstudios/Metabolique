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

        [JsonRequired] public string answerQuestion5a { get; set; } = null!;

        [JsonRequired] public string answerQuestion5b { get; set; } = null!;

        [JsonRequired] public string answerQuestion5c { get; set; } = null!;

        [JsonRequired] public string answerQuestion5d { get; set; } = null!;

        [JsonRequired] public string answerQuestion5e { get; set; } = null!;

        [JsonRequired] public string answerQuestion5f { get; set; } = null!;

        [JsonRequired] public string answerQuestion5g { get; set; } = null!;

        [JsonRequired] public string answerQuestion5h { get; set; } = null!;

        [JsonRequired] public string answerQuestion5i { get; set; } = null!;

        [JsonRequired] public string answerQuestion5j { get; set; } = null!;

        [JsonRequired] public string answerQuestion6 { get; set; } = null!;

        [JsonRequired] public string answerQuestion7 { get; set; } = null!;

        [JsonRequired] public string answerQuestion8 { get; set; } = null!;

        [JsonRequired] public string answerQuestion9 { get; set; } = null!;
    }
}
