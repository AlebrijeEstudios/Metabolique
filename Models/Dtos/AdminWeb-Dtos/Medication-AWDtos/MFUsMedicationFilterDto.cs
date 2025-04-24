namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos
{
    public class MFUsMedicationFilterDto
    {
        public Guid doctorID { get; set; }

        public Guid? accountID { get; set; }

        public string? uiemID { get; set; }

        public string? username { get; set; }

        public int? month { get; set; }

        public int? year { get; set; }

        public string? sex { get; set; }

        public string? protocolToFollow { get; set; }

        public string? statusAdherence { get; set; }
    }
}
