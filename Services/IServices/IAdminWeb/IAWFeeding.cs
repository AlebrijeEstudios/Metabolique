using AppVidaSana.Models.Dtos.AdminWeb_Dtos;

namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWFeeding
    {
        Task<List<FeedingsAdminDto>> GetFeedingsAsync(Guid accountID, int page, CancellationToken cancellationToken);

        Task<List<FeedingsAdminDto>> GetFilterFeedingsAsync(Guid accountID, int page, DateOnly dateInitial, DateOnly dateFinal, CancellationToken cancellationToken);

        Task<byte[]> ExportAllToCsvAsync(Guid accountID, CancellationToken cancellationToken);

        Task<byte[]> ExportFilteredToCsvAsync(Guid accountID, DateOnly dateInitial, DateOnly dateFinal, CancellationToken cancellationToken);
    }
}
