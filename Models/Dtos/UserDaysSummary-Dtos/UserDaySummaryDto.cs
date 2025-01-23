using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.UserDaysSummary_Dtos
{
    public class UserDaySummaryDto
    {
        [JsonRequired] public string userName { get; set; } = null!;

        [JsonRequired] public CaloriesSummaryDto calories { get; set; } = null!;

        [JsonRequired] public int timeExercise { get; set; }

        public int? timeSleep { get; set; }

        [JsonRequired] public MedicationSummaryDto medications { get; set; } = null!;
    }
}
