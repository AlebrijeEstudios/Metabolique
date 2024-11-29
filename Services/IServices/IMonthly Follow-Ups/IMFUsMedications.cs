using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Medications_Dtos;

namespace AppVidaSana.Services.IServices.IMonthly_Follow_Ups
{
    public interface IMFUsMedications
    {
        Task<RetrieveResponsesMedicationsDto?> RetrieveAnswersAsync(Guid accountID, int month, int year, CancellationToken cancellationToken);
        
        Task<RetrieveResponsesMedicationsDto?> SaveAnswersAsync(SaveResponsesMedicationsDto values, CancellationToken cancellationToken);

        Task<RetrieveResponsesMedicationsDto?> UpdateAnswersAsync(UpdateResponsesMedicationsDto values, CancellationToken cancellationToken);

        bool Save();

    }
}
