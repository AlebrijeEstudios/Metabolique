using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos;

namespace AppVidaSana.ProducesResponseType.AdminWeb.Medication
{
    public class GetSideEffectsResponse
    {
        public string message { get; set; } = "Ok.";

        public List<AllSideEffectsPerUserDto> sideEffects { get; set; } = null!;
    }
}
