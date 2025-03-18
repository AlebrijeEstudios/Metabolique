using AppVidaSana.Services.IServices;
using AppVidaSana.Services.IServices.IAdminWeb;
using System.IO.Compression;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWExportToZipService : IExportToZip
    {
        private readonly IAWFeeding _feedingService;

        public AWExportToZipService(IAWFeeding feedingService)
        {
            _feedingService = feedingService;
        }

        public async Task<byte[]> GenerateAllSectionsZipAsync(CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                async Task<byte[]> CreateSectionZip(Dictionary<string, byte[]> files)
                {
                    using var sectionStream = new MemoryStream();
                    using (var sectionZip = new ZipArchive(sectionStream, ZipArchiveMode.Create, true))
                    {
                        foreach (var file in files)
                        {
                            var entry = sectionZip.CreateEntry(file.Key, CompressionLevel.Optimal);
                            using var entryStream = entry.Open();
                            await entryStream.WriteAsync(file.Value, 0, file.Value.Length, cancellationToken);
                        }
                    }
                    return sectionStream.ToArray();
                }

                async Task AddSectionToMainZip(string zipName, Dictionary<string, byte[]> sectionFiles)
                {
                    var sectionZipBytes = await CreateSectionZip(sectionFiles);
                    var entry = mainZip.CreateEntry(zipName, CompressionLevel.Optimal);
                    using var entryStream = entry.Open();
                    await entryStream.WriteAsync(sectionZipBytes, 0, sectionZipBytes.Length, cancellationToken);
                }


                var sectionFeeding = new Dictionary<string, byte[]>
                {
                    { "All_Feedings.csv", await _feedingService.ExportAllFeedingsAsync(cancellationToken) },
                    { "All_CaloriesConsumedPerPatient.csv", await _feedingService.ExportAllCaloriesConsumedAsync(cancellationToken) },
                    { "All_CaloriesRequiredPerDaysPerPatient.csv", await _feedingService.ExportAllCaloriesRequiredPerDaysAsync(cancellationToken)},
                    { "All_UserCalories.csv", await _feedingService.ExportAllUserCaloriesAsync(cancellationToken)},
                    { "All_MFUsFeeding.csv", await _feedingService.ExportAllMFUsFeedingAsync(cancellationToken)}
                };

                string dateSuffix = DateTime.UtcNow.ToString("yyyy-MM-dd");

                await AddSectionToMainZip("Section_Feedings_{dateSuffix}.zip", sectionFeeding);
            }

            return mainMemoryStream.ToArray();
        }
    }
}
