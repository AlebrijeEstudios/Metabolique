using AppVidaSana.Models.Dtos.AdminWeb_Dtos.GeneralFilters_AWDtos;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos
{
    public class PeriodMedicationsFilterDto
    {
        public GeneralPatientFilterDto? patientFilter { get; set; }

        public GeneralMonthYearFilterDto? monthYearFilter { get; set; }

        public GeneralDatesFilterDto? datesFilter { get; set; }

        public string? nameMedication { get; set; }

        public bool? status { get; set; }
    }
}
