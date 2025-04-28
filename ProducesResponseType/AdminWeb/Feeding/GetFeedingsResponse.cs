using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Feeding
{
    public class GetFeedingsResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllFeedsOfAUserDto> feedings { get; set; } = null!;
    }
}
