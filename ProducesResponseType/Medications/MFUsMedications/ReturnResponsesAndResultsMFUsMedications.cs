using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Medications_Dtos;

namespace AppVidaSana.ProducesResponseType.Medications.MFUsMedications
{
    public class ReturnResponsesAndResultsMFUsMedications
    {
        public string message { get; set; } = "Ok.";

        public RetrieveResponsesMedicationsDto mfus { get; set; } = null!;
    }
}
