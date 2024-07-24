using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.Drugs
{
    public class DrugsConsumedDto
    {
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public DateOnly drugsDateHabit { get; set; }

        [JsonRequired] public int cigarettesSmoked { get; set; }

        [JsonRequired] public string predominantEmotionalState { get; set; } = null!;

    }
}
