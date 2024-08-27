using AppVidaSana.Models.Dtos.Medication_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IMedication
    {

        public MedicationsAndValuesGraphicDto GetMedications(Guid id, DateOnly date);

        public InfoMedicationDto AddMedication(AddMedicationUseDto medication);

        public InfoMedicationDto UpdateMedication(UpdateMedicationUseDto values); 

        public void UpdateStatusMedication(UpdateMedicationStatusDto value);

        public string DeleteAMedication(Guid id, DateOnly date);

        bool Save();

    }
}
