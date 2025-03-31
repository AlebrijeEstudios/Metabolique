using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.Models.Dtos.Feeding_Dtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb
{
    public class GetFeedingsResponse
    {
        public string message { get; set; } = "Ok.";

        public List<FoodsConsumedAdminDto> feedings { get; set; } = null!;
    }
}
