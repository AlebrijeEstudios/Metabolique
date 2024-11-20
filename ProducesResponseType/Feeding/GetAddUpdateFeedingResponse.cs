using AppVidaSana.Models.Dtos.Feeding_Dtos;

namespace AppVidaSana.ProducesResponseType.Feeding
{
    public class GetAddUpdateFeedingResponse
    {
        public string message { get; set; } = "Ok.";

        public UserFeedsDto feeding { get; set; } = null!;
    }
}
