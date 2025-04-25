using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Exercise_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Exercise
{
    public class GetActiveMinutesResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllActiveMinutesPerExerciseDto> actMin { get; set; } = null!;
    }
}
