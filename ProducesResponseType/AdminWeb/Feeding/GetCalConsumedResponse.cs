using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Feeding
{
    public class GetCalConsumedResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllCaloriesConsumedPerUserDto> calConsumed { get; set; } = null!;
    }
}
