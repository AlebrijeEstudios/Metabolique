using AppVidaSana.Models.Dtos.Medication_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IMedication
    {

        public List<InfoMedicationDto> GetMedications(Guid id, DateOnly date);

        public List<InfoMedicationDto> AddMedication(AddMedicationUseDto medication);

        public List<InfoMedicationDto> UpdateMedication(UpdateMedicationUseDto values);

        public List<InfoMedicationDto> DeleteAMedication(Guid id, DateOnly date);

        bool Save();

    }
}
