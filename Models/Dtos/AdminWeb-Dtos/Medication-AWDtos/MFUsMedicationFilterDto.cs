using AppVidaSana.Models.Dtos.AdminWeb_Dtos.GeneralFilters_AWDtos;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos
{
    public class MFUsMedicationFilterDto
    {
        public GeneralPatientFilterDto? patientFilter { get; set; }

        public GeneralMonthYearFilterDto? monthYearFilter { get; set; }

        public string? statusAdherence { get; set; }
    }
}
