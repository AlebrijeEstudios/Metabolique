using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Habits_Dtos;

namespace AppVidaSana.ProducesResponseType.Habits
{
    public class ReturnGetSleepingHours
    {
        public string message { get; set; } = "Ok.";

        public List<GetSleepingHoursDto> hoursSleep { get; set; } = null!;
    }
}
