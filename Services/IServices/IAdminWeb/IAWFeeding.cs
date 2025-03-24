using AppVidaSana.Models.Dtos.AdminWeb_Dtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWFeeding
    {
        Task<List<FeedingsAdminDto>> GetFeedingsAsync(Guid accountID, int page, CancellationToken cancellationToken);

        Task<List<FeedingsAdminDto>> GetFilterFeedingsAsync(Guid accountID, int page, DateOnly dateInitial, DateOnly dateFinal, CancellationToken cancellationToken);

        Task<byte[]> ExportFeedingsAsync(Guid accountID, CancellationToken cancellationToken);

        Task<byte[]> ExportFilteredFeedingsAsync(Guid accountID, DateOnly dateInitial, DateOnly dateFinal, CancellationToken cancellationToken);

        Task<byte[]> ExportAllFeedingsAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllFoodsConsumedPerFeedingAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllCaloriesConsumedAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllCaloriesRequiredPerDaysAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllUserCaloriesAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllMFUsFeedingAsync(CancellationToken cancellationToken);
    }
}
