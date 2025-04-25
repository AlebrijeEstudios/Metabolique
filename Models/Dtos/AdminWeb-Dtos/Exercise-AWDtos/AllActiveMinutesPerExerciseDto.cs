using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Exercise_AWDtos
{
    public class AllActiveMinutesPerExerciseDto
    {
        [JsonRequired] public Guid timeSpentID { get; set; }

        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public string username { get; set; } = null!;

        [JsonRequired] public DateOnly dateExercise { get; set; }

        [JsonRequired] public int totalTimeSpent { get; set; }
    }
}
