using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.Models.Dtos.Feeding_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IFeeding
    {
        Task<UserFeedsDto> GetFeedingAsync(Guid userFeedID, CancellationToken cancellationToken);

        Task<InfoGeneralFeedingDto> GetInfoGeneralFeedingAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken);

        Task<List<FeedingsAdminDto>> GetFeedingsAsync(Guid accountID, int page, CancellationToken cancellationToken);

        Task<List<FeedingsAdminDto>> GetFilterFeedingsAsync(Guid accountID, int page, DateOnly dateInitial, DateOnly dateFinal, CancellationToken cancellationToken);

        Task<byte[]> ExportAllToCsvAsync(Guid accountID, CancellationToken cancellationToken);

        Task<byte[]> ExportFilteredToCsvAsync(Guid accountID, DateOnly dateInitial, DateOnly dateFinal, CancellationToken cancellationToken);

        Task<UserFeedsDto> AddFeedingAsync(AddFeedingDto values, CancellationToken cancellationToken);

        Task<UserFeedsDto> UpdateFeedingAsync(UpdateFeedingDto values, CancellationToken cancellationToken);

        Task<bool> DeleteFeedingAsync(Guid userFeedID, CancellationToken cancellationToken);

        bool Save();
    }
}
