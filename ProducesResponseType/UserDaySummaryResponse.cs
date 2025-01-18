using AppVidaSana.Models.Dtos.UserDaysSummary_Dtos;

namespace AppVidaSana.ProducesResponseType
{
    public class UserDaySummaryResponse
    {
        public string message { get; set; } = "Ok.";

        public UserDaySummaryDto summary { get; set; } = null!;
    }
}
