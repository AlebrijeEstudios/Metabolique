using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Exercise_Dtos
{
    public class SaveResultsExerciseDto
    {
        [JsonRequired] public Guid monthlyFollowUpID { get; set; }

        [JsonRequired] public float actWalking { get; set; }

        [JsonRequired] public float actModerate { get; set; }

        [JsonRequired] public float actVigorous { get; set; }

        [JsonRequired] public float totalMET { get; set; }

        [JsonRequired] public string sedentaryBehavior { get; set; } = null!;

        [JsonRequired] public string levelAF { get; set; } = null!;
    }
}
