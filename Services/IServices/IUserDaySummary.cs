using AppVidaSana.Models.Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IUserDaySummary
    {
        Task<UserDaySummaryDto> GetUserDaySummaryAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken);
    }
}
