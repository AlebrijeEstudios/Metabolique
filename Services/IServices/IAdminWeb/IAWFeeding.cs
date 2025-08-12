using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWFeeding
    {
        Task<List<AllFeedsOfAUserDto>> GetAllFeedsOfAUserAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllFoodsConsumedPerUserFeedDto>> GetAllFoodsConsumedPerUserFeedAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllUserCaloriesDto>> GetAllUserCaloriesAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllMFUsFeedingPerUserDto>> GetMFUsFeedingAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);


        Task<byte[]> ExportAllFeedingsAsync(FilterAdminDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllFoodsConsumedPerFeedingAsync(FilterAdminDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllUserCaloriesAsync(FilterAdminDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllMFUsFeedingAsync(FilterAdminDto? filter, CancellationToken cancellationToken);


        Task<List<AllCaloriesConsumedPerUserDto>> GetAllCaloriesConsumedPerUserAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);

        Task<List<AllCaloriesRequiredPerDaysDto>> GetAllCaloriesRequiredPerDaysAsync(FilterAdminDto filter, int page, CancellationToken cancellationToken);

        Task<byte[]> ExportAllCaloriesConsumedAsync(FilterAdminDto? filter, CancellationToken cancellationToken);

        Task<byte[]> ExportAllCaloriesRequiredPerDaysAsync(FilterAdminDto? filter, CancellationToken cancellationToken);
    }
}
