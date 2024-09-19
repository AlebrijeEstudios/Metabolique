using AppVidaSana.Models.Dtos.Medication_Dtos;

namespace AppVidaSana.ProducesResponseType.Medications.SideEffects
{
    public class ReturnSideEffect
    {
        public string message { get; set; } = "Ok.";

        public SideEffectsListDto sideEffect { get; set; } = null!;
    }
}
