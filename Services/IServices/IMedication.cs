using AppVidaSana.Models.Dtos.Medication_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IMedication
    {

        public MedicationsAndValuesGraphicDto GetMedications(Guid id, DateOnly date);

        public List<InfoMedicationDto> AddMedication(AddMedicationUseDto medication);

        public List<InfoMedicationDto> UpdateMedication(UpdateMedicationUseDto values);

        public void UpdateStatusMedication(UpdateMedicationStatusDto value);

        public List<InfoMedicationDto> DeleteAMedication(Guid id, DateOnly date);

        bool Save();

    }
}
