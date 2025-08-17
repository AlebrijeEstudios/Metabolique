namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos
{
    public class FilterAdminDto
    {
        public Guid? doctorID { get; set; }

        public string? role { get; set; }

        public Guid? accountID { get; set; }

        public string? username { get; set; }

        public string? uiemID { get; set; }

        public string? sex { get; set; }

        public string? protocolToFollow { get; set; }

        public int? month { get; set; }

        public int? year { get; set; }

        public DateOnly? startDate { get; set; }

        public DateOnly? endDate { get; set; }

        public string? typeExercise { get; set; }

        public string? intensityExercise { get; set; }


        public string? dailyMeal { get; set; }


        public string? perceptionRelax { get; set; }

        public string? predominatEmotionalState { get; set; }


        public string? nameMedication { get; set; }

        public bool? status { get; set; }

        public string? statusAdherence { get; set; }
    }
}
