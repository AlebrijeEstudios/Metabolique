using AppVidaSana.Models.Dtos.AdminWeb_Dtos.GeneralFilters_AWDtos;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Exercise_AWDtos
{
    public class ExerciseFilterDto
    {
        public GeneralPatientFilterDto? patientFilter { get; set; }

        public GeneralMonthYearFilterDto? monthYearFilter { get; set; }

        public GeneralDatesFilterDto? datesFilter { get; set; }

        public string? typeExercise { get; set; }

        public string? intensityExercise { get; set; }
    }
}
