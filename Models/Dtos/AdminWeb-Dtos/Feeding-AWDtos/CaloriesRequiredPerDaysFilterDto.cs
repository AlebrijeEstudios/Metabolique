using AppVidaSana.Models.Dtos.AdminWeb_Dtos.GeneralFilters_AWDtos;

namespace AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos
{
    public class CaloriesRequiredPerDaysFilterDto
    {
        public GeneralPatientFilterDto? patientFilter { get; set; }

        public GeneralDatesFilterDto? datesFilter { get; set; }
    }
}
