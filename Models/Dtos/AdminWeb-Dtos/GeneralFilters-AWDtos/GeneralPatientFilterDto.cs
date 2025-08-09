namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.GeneralFilters_AWDtos
{
    public class GeneralPatientFilterDto
    {
        public Guid? doctorID { get; set; }

        public Guid? accountID { get; set; }

        public string? username { get; set; }

        public string? uiemID { get; set; }

        public string? sex { get; set; }

        public string? protocolToFollow { get; set; }
    }
}
