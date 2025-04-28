using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Feeding
{
    public class GetUserCaloriesResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllUserCaloriesDto> userCal { get; set; } = null!;
    }
}
