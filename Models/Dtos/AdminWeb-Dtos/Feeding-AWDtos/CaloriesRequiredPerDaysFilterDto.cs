using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos
{
    public class CaloriesRequiredPerDaysFilterDto
    {
        public Guid doctorID { get; set; }

        public Guid? accountID { get; set; }

        public string? username { get; set; }

        public string? uiemID { get; set; }

        public DateOnly? startDate { get; set; }

        public DateOnly? endDate { get; set; }

        public string? sex { get; set; }

        public string? protocolToFollow { get; set; }
    }
}
