using AppVidaSana.Models.Dtos.AdminWeb_Dtos.GeneralFilters_AWDtos;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos
{
    public class PatientFilterDto
    {
        public GeneralPatientFilterDto? patientFilter { get; set; }

        public GeneralMonthYearFilterDto? monthYearFilter { get; set; }
    }
}
