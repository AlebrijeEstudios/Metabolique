using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos
{
    public class RetrieveResponsesExerciseDto
    {
        [JsonRequired] public Guid monthlyFollowUpID { get; set; }

        [JsonRequired] public string month { get; set; } = null!;

        [JsonRequired] public int year { get; set; }

        [JsonRequired] public int question1 { get; set; }

        [JsonRequired] public int question2 { get; set; }

        [JsonRequired] public int question3 { get; set; }

        [JsonRequired] public int question4 { get; set; }

        [JsonRequired] public int question5 { get; set; }

        [JsonRequired] public int question6 { get; set; }

        [JsonRequired] public int question7 { get; set; }

        [JsonRequired] public float actWalking { get; set; }

        [JsonRequired] public float actModerate { get; set; }

        [JsonRequired] public float actVigorous { get; set; }

        [JsonRequired] public float totalMET { get; set; }

        [JsonRequired] public string sedentaryBehavior { get; set; } = null!;

        [JsonRequired] public string levelAF { get; set; } = null!;
    }
}
