using AppVidaSana.Models.Dtos.Habits_Dtos;

namespace AppVidaSana.Services.IServices.IHabits
{
    public interface IHabitsGeneral
    {
        Task<ReturnInfoHabitsDto> GetInfoGeneralHabitsAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken);
    }
}
