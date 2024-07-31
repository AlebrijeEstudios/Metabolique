using AppVidaSana.Models.Dtos.Medication_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IMedication
    {

        public List<ReturnInfoMedicationDto> GetAccount(Guid id, DateOnly date);

        public List<ReturnInfoMedicationDto> AddMedication(AddUpdateMedicationUseDto medication);

        public List<ReturnInfoMedicationDto> UpdateMedication(AddUpdateMedicationUseDto values);

        public List<ReturnInfoMedicationDto> DeleteMedication(Guid id);

        bool Save();

    }
}
