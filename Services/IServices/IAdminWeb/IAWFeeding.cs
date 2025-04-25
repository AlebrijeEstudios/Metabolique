using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWFeeding
    {
        Task<List<AllFeedsOfAUserDto>> GetAllFeedsOfAUserAsync(UserFeedFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllFoodsConsumedPerUserFeedDto>> GetAllFoodsConsumedPerUserFeedAsync(UserFeedFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllCaloriesConsumedPerUserDto>> GetAllCaloriesConsumedPerUserAsync(CaloriesConsumedFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllCaloriesRequiredPerDaysDto>> GetAllCaloriesRequiredPerDaysAsync(CaloriesRequiredPerDaysFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllUserCaloriesDto>> GetAllUserCaloriesAsync(PatientFilterDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllMFUsFeedingPerUserDto>> GetMFUsFeedingAsync(PatientFilterDto filter, int page, CancellationToken cancellationToken);


        Task<byte[]> ExportAllFeedingsAsync(UserFeedFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllFoodsConsumedPerFeedingAsync(UserFeedFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllCaloriesConsumedAsync(CaloriesConsumedFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllCaloriesRequiredPerDaysAsync(CaloriesRequiredPerDaysFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllUserCaloriesAsync(PatientFilterDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllMFUsFeedingAsync(PatientFilterDto? filter, CancellationToken cancellationToken);
    }
}
