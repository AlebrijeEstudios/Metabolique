using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Exercise_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Exercise
{
    public class GetExercisesResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllExercisesPerUserDto> exercises { get; set; } = null!;
    }
}
