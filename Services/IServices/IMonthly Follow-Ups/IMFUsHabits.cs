using AppVidaSana.Models.Dtos.Monthly_Follow_Ups_Dtos.Habits_Dtos;

namespace AppVidaSana.Services.IServices.IMonthly_Follow_Ups
{
    public interface IMFUsHabits
    {
        Task<RetrieveResponsesHabitsDto?> RetrieveAnswersAsync(Guid accountID, int month, int year, CancellationToken cancellationToken);
        
        Task<RetrieveResponsesHabitsDto?> SaveAnswersAsync(SaveResponsesHabitsDto values, CancellationToken cancellationToken);

        Task<RetrieveResponsesHabitsDto?> UpdateAnswersAsync(UpdateResponsesHabitsDto values, CancellationToken cancellationToken);

        bool Save();

    }
}
