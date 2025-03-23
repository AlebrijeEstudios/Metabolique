namespace AppVidaSana.Services.IServices
{
    public interface IExportToZip
    {
        Task<byte[]> GenerateAllSectionsZipAsync(CancellationToken cancellationToken);
    }
}
