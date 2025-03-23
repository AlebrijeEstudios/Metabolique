using AppVidaSana.Services.IServices;
using AppVidaSana.Services.IServices.IAdminWeb;
using System.IO.Compression;

namespace AppVidaSana.Services.AdminWeb
{
    public class AWExportToZipService : IExportToZip
    {
        private readonly IAWPatients _patientsService;
        private readonly IAWFeeding _feedingService;
        private readonly IAWMedication _medicationService;
        private readonly IAWExercise _exerciseService;
        private readonly IAWHabits _habitService;

        public AWExportToZipService(IAWPatients patientsService, IAWFeeding feedingService, IAWMedication medicationService, IAWExercise exerciseService, IAWHabits habitService)
        {
            _patientsService = patientsService;
            _feedingService = feedingService;
            _medicationService = medicationService;
            _exerciseService = exerciseService;
            _habitService = habitService;
        }

        public async Task<byte[]> GenerateAllSectionsZipAsync(CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                async Task AddSectionToMainZip(string zipName, Dictionary<string, byte[]> sectionFiles)
                {
                    string folderName = Path.GetFileNameWithoutExtension(zipName);

                    var sectionZipBytes = await CreateSectionZip(folderName, sectionFiles);

                    var entry = mainZip.CreateEntry(zipName, CompressionLevel.Optimal);
                    using var entryStream = entry.Open();
                    await entryStream.WriteAsync(sectionZipBytes, 0, sectionZipBytes.Length, cancellationToken);
                }

                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");

                var sectionPatients = new Dictionary<string, byte[]>
                {
                    { $"All_Patients_{dateSuffix}.csv", await  _patientsService.ExportAllPatientsAsync(cancellationToken) }
                };

                var sectionFeeding = new Dictionary<string, byte[]>
                {
                    { $"All_Feedings_{dateSuffix}.csv", await _feedingService.ExportAllFeedingsAsync(cancellationToken) },
                    { $"All_CaloriesConsumedPerPatient_{dateSuffix}.csv", await _feedingService.ExportAllCaloriesConsumedAsync(cancellationToken) },
                    { $"All_CaloriesRequiredPerDaysPerPatient_{dateSuffix}.csv", await _feedingService.ExportAllCaloriesRequiredPerDaysAsync(cancellationToken)},
                    { $"All_UserCalories_{dateSuffix}.csv", await _feedingService.ExportAllUserCaloriesAsync(cancellationToken)},
                    { $"All_MFUsFeeding_{dateSuffix}.csv", await _feedingService.ExportAllMFUsFeedingAsync(cancellationToken) }
                };

                var sectionMedication = new Dictionary<string, byte[]>
                {
                    { $"All_PeriodsMedications_{dateSuffix}.csv", await _medicationService.ExportAllPeriodsMedicationsAsync(cancellationToken) },
                    { $"All_DaysConsumedOfMedications_{dateSuffix}.csv", await _medicationService.ExportAllDaysConsumedOfMedAsync(cancellationToken) },
                    { $"All_ConsumptionTimes_{dateSuffix}.csv", await _medicationService.ExportAllConsumptionTimesAsync(cancellationToken)},
                    { $"All_SideEffects_{dateSuffix}.csv", await _medicationService.ExportAllSideEffectsAsync(cancellationToken)},
                    { $"All_MFUsMedication_{dateSuffix}.csv", await _medicationService.ExportAllMFUsMedicationAsync(cancellationToken) }
                };

                var sectionExercise = new Dictionary<string, byte[]>
                {
                    { $"All_Exercises_{dateSuffix}.csv", await _exerciseService.ExportAllExercisesAsync(cancellationToken) },
                    { $"All_ActivesMinutes_{dateSuffix}.csv", await _exerciseService.ExportAllActivesMinutesAsync(cancellationToken) },
                    { $"All_MFUsExercise_{dateSuffix}.csv", await _exerciseService.ExportAllMFUsExerciseAsync(cancellationToken) }
                };

                var sectionHabit = new Dictionary<string, byte[]>
                {
                    { $"All_HabitsDrink_{dateSuffix}.csv", await _habitService.ExportAllHabitsDrinkAsync(cancellationToken) },
                    { $"All_HabitsDrugs_{dateSuffix}.csv", await _habitService.ExportAllHabitsDrugsAsync(cancellationToken) },
                    { $"All_HabitsSleep_{dateSuffix}.csv", await _habitService.ExportAllHabitsSleepAsync(cancellationToken) },
                    { $"All_MFUsHabits_{dateSuffix}.csv", await _habitService.ExportAllMFUsHabitsAsync(cancellationToken) }
                };

                await AddSectionToMainZip($"Section_Patients_{dateSuffix}.zip", sectionPatients);
                await AddSectionToMainZip($"Section_Feedings_{dateSuffix}.zip", sectionFeeding);
                await AddSectionToMainZip($"Section_Medication_{dateSuffix}.zip", sectionMedication);
                await AddSectionToMainZip($"Section_Exercise_{dateSuffix}.zip", sectionExercise);
                await AddSectionToMainZip($"Section_Habit_{dateSuffix}.zip", sectionHabit);
            }

            return mainMemoryStream.ToArray();
        }
    
        private async Task<byte[]> CreateSectionZip(string folderName, Dictionary<string, byte[]> sectionFiles)
        {
            using var memoryStream = new MemoryStream();
            using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
            {
                foreach (var file in sectionFiles)
                {
                    string filePathInZip = $"{folderName}/{file.Key}"; 

                    var entry = archive.CreateEntry(filePathInZip, CompressionLevel.Optimal);
                    using var entryStream = entry.Open();
                    await entryStream.WriteAsync(file.Value, 0, file.Value.Length);
                }
            }

            return memoryStream.ToArray();
        }
    
    }
}
