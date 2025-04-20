using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWFeeding
    {
        Task<List<AllFeedsOfAUserDto>> GetAllFeedsOfAUserAsync(UserFeedFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllFoodsConsumedPerUserFeedDto>> GetAllFoodsConsumedPerUserFeedAsync(UserFeedFilterDto filter, int page, CancellationToken cancellationToken);

        Task<byte[]> ExportAllFeedingsAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllFoodsConsumedPerFeedingAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllCaloriesConsumedAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllCaloriesRequiredPerDaysAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllUserCaloriesAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllMFUsFeedingAsync(CancellationToken cancellationToken);
    }
}
