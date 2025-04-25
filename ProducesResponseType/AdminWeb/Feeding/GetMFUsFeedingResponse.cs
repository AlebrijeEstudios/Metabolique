using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Feeding
{
    public class GetMFUsFeedingResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllMFUsFeedingPerUserDto> mfu { get; set; } = null!;
    }
}
