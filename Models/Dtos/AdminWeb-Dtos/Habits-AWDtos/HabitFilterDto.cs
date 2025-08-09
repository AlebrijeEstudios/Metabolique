using AppVidaSana.Models.Dtos.AdminWeb_Dtos.GeneralFilters_AWDtos;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Habits_AWDtos
{
    public class HabitFilterDto
    {
        public GeneralPatientFilterDto? patientFilter { get; set; }

        public GeneralMonthYearFilterDto? monthYearFilter { get; set; }

        public GeneralDatesFilterDto? datesFilter { get; set; }

        public string? perceptionRelax { get; set; }

        public string? predominatEmotionalState { get; set; }
    }
}
