namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos
{
    public class UserFeedFilterDto
    {
        public Guid doctorID { get; set; }

        public Guid? accountID { get; set; }

        public string? username { get; set; }

        public string? uiemID { get; set; }
        
        public int? month { get; set; }

        public int? year { get; set; }

        public string? sex { get; set; }

        public string? protocolToFollow { get; set; }

        public string? dailyMeal { get; set; }

        public DateOnly? startDate { get; set; }

        public DateOnly? endDate { get; set; }
    }
}
