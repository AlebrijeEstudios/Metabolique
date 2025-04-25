using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Exercise_AWDtos
{
    public class AllExercisesPerUserDto
    {
        [JsonRequired] public Guid exerciseID { get; set; }

        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public string username { get; set; } = null!;

        [JsonRequired] public DateOnly dateExercise { get; set; }

        [JsonRequired] public string typeExercise { get; set; } = null!;

        [JsonRequired] public string intensityExercise { get; set; } = null!;

        [JsonRequired] public int timeSpent { get; set; }
    }
}
