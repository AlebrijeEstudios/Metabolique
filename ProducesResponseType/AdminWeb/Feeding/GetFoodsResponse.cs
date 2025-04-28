using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Feeding
{
    public class GetFoodsResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllFoodsConsumedPerUserFeedDto> foods { get; set; } = null!;
    }
}
