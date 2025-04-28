using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Exercise_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Exercise
{
    public class GetMFUsExerciseResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllMFUsExercisePerUserDto> mfu { get; set; } = null!;
    }
}
