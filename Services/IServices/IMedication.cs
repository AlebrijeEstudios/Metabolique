using AppVidaSana.Models.Dtos.Medication_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IMedication
    {
        public Task<MedicationsAndValuesGraphicDto> GetMedicationsAsync(Guid accountID, DateOnly dateActual, CancellationToken cancellationToken);

        public Task<InfoMedicationDto?> AddMedicationAsync(AddMedicationUseDto values, CancellationToken cancellationToken);

        public Task<InfoMedicationDto?> UpdateMedicationAsync(UpdateMedicationUseDto values, CancellationToken cancellationToken);

        public Task UpdateStatusMedicationAsync(UpdateMedicationStatusDto value, CancellationToken cancellationToken);

        public Task<string> DeleteAMedicationAsync(Guid id, DateOnly date, CancellationToken cancellationToken);

        bool Save();
    }
}
