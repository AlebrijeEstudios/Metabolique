using AppVidaSana.Models.Dtos.Feeding_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IFeeding
    {
        Task<UserFeedsDto> GetFeedingAsync(Guid userFeedID, CancellationToken cancellationToken);

        Task<InfoGeneralFeedingDto> GetInfoGeneralFeedingAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken);

        Task<UserFeedsDto> AddFeedingAsync(AddFeedingDto values, List<FoodsConsumedDto> foodsConsumed, CancellationToken cancellationToken);

        Task<UserFeedsDto> UpdateFeedingAsync(UpdateFeedingDto values, List<FoodsConsumedDto> foodsConsumed, CancellationToken cancellationToken);

        Task<bool> DeleteFeedingAsync(Guid userFeedID, CancellationToken cancellationToken);

        bool Save();

    }
}
