using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Habits_Dtos.Drink
{
    public class DrinkHabitDto
    { 
        [JsonRequired] public Guid accountID { get; set; }

        [JsonRequired] public DateOnly dateRegister { get; set; }

        public int? amountConsumed { get; set; }

    }
}
