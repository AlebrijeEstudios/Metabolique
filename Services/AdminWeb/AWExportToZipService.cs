using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Exercise_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Habits_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;
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
                    { $"All_Patients_{dateSuffix}.csv", await  _patientsService.ExportPatientsAsync(null, cancellationToken) }
                };

                var sectionFeeding = new Dictionary<string, byte[]>
                {
                    { $"All_InfoFeedings_{dateSuffix}.csv", await _feedingService.ExportAllFeedingsAsync(null, cancellationToken) },
                    { $"All_FoodsConsumedPerFeedingPerPatient_{dateSuffix}.csv", await _feedingService.ExportAllFoodsConsumedPerFeedingAsync(null, cancellationToken) },
                    { $"All_TotalCaloriesConsumedPerPatientPerDay_{dateSuffix}.csv", await _feedingService.ExportAllCaloriesConsumedAsync(null, cancellationToken) },
                    { $"All_CaloriesRequiredPerDaysPerPatient_{dateSuffix}.csv", await _feedingService.ExportAllCaloriesRequiredPerDaysAsync(null, cancellationToken)},
                    { $"All_CaloriesRequiredPerPatient_{dateSuffix}.csv", await _feedingService.ExportAllUserCaloriesAsync(null, cancellationToken)},
                    { $"All_MFUsFeeding_{dateSuffix}.csv", await _feedingService.ExportAllMFUsFeedingAsync(null, cancellationToken) }
                };

                var sectionMedication = new Dictionary<string, byte[]>
                {
                    { $"All_PeriodsMedications_{dateSuffix}.csv", await _medicationService.ExportAllPeriodsMedicationsAsync(cancellationToken) },
                    { $"All_DaysConsumedOfMedications_{dateSuffix}.csv", await _medicationService.ExportAllDaysConsumedOfMedAsync(cancellationToken) },
                    { $"All_ConsumptionTimes_{dateSuffix}.csv", await _medicationService.ExportAllConsumptionTimesAsync(cancellationToken)},
                    { $"All_SideEffects_{dateSuffix}.csv", await _medicationService.ExportAllSideEffectsAsync(null, cancellationToken)},
                    { $"All_MFUsMedication_{dateSuffix}.csv", await _medicationService.ExportAllMFUsMedicationAsync(null, cancellationToken) }
                };

                var sectionExercise = new Dictionary<string, byte[]>
                {
                    { $"All_Exercises_{dateSuffix}.csv", await _exerciseService.ExportAllExercisesAsync(null, cancellationToken) },
                    { $"All_ActivesMinutes_{dateSuffix}.csv", await _exerciseService.ExportAllActivesMinutesAsync(null, cancellationToken) },
                    { $"All_MFUsExercise_{dateSuffix}.csv", await _exerciseService.ExportAllMFUsExerciseAsync(null, cancellationToken) }
                };

                var sectionHabit = new Dictionary<string, byte[]>
                {
                    { $"All_HabitsDrink_{dateSuffix}.csv", await _habitService.ExportAllHabitsDrinkAsync(null, cancellationToken) },
                    { $"All_HabitsDrugs_{dateSuffix}.csv", await _habitService.ExportAllHabitsDrugsAsync(null, cancellationToken) },
                    { $"All_HabitsSleep_{dateSuffix}.csv", await _habitService.ExportAllHabitsSleepAsync(null, cancellationToken) },
                    { $"All_MFUsHabits_{dateSuffix}.csv", await _habitService.ExportAllMFUsHabitsAsync(null, cancellationToken) }
                };

                await AddSectionToMainZip($"Section_Patients_{dateSuffix}.zip", sectionPatients);
                await AddSectionToMainZip($"Section_Feedings_{dateSuffix}.zip", sectionFeeding);
                await AddSectionToMainZip($"Section_Medication_{dateSuffix}.zip", sectionMedication);
                await AddSectionToMainZip($"Section_Exercise_{dateSuffix}.zip", sectionExercise);
                await AddSectionToMainZip($"Section_Habit_{dateSuffix}.zip", sectionHabit);
            }

            return mainMemoryStream.ToArray();
        }


        public async Task<byte[]> GenerateOnlyPatientsZipAsync(PatientFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _patientsService.ExportPatientsAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"Patients_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_Patients_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }


        public async Task<byte[]> GenerateOnlyFeedingsZipAsync(UserFeedFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _feedingService.ExportAllFeedingsAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"InfoFeedings_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_InfoFeedings_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyFoodsConsumedPerFeedingZipAsync(UserFeedFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _feedingService.ExportAllFoodsConsumedPerFeedingAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"FoodsConsumedPerFeedingPerPatient_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_FoodsConsumedPerFeedingPerPatient_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyCaloriesConsumedZipAsync(CaloriesConsumedFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _feedingService.ExportAllCaloriesConsumedAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"TotalCaloriesConsumedPerPatientPerDay_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_TotalCaloriesConsumedPerPatientPerDay_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyCaloriesRequiredPerDaysZipAsync(CaloriesRequiredPerDaysFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _feedingService.ExportAllCaloriesRequiredPerDaysAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"CaloriesRequiredPerDaysPerPatient_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_CaloriesRequiredPerDaysPerPatient_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyUserCaloriesZipAsync(PatientFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _feedingService.ExportAllUserCaloriesAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"CaloriesRequiredPerPatient_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_CaloriesRequiredPerPatient_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyMFUsFeedingZipAsync(PatientFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _feedingService.ExportAllMFUsFeedingAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"MFUsFeeding_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_MFUsFeeding_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }


        public async Task<byte[]> GenerateOnlyPeriodMedicationsZipAsync(PeriodMedicationsFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _medicationService.ExportAllPeriodMedicationsAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"PeriodsMedications_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_PeriodsMedications_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlySideEffectsZipAsync(SideEffectsFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _medicationService.ExportAllSideEffectsAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"SideEffects_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_SideEffects_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyMFUsMedicationZipAsync(MFUsMedicationFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _medicationService.ExportAllMFUsMedicationAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"MFUsMedication_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_MFUsMedication_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }


        public async Task<byte[]> GenerateOnlyExercisesZipAsync(ExerciseFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _exerciseService.ExportAllExercisesAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"Exercises_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_Exercises_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyActivesMinutesZipAsync(ActiveMinutesFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _exerciseService.ExportAllActivesMinutesAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"ActivesMinutes_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_ActivesMinutes_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyMFUsExerciseZipAsync(PatientFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _exerciseService.ExportAllMFUsExerciseAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"MFUsExercise_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_MFUsExercise_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }


        public async Task<byte[]> GenerateOnlyHabitsDrinkZipAsync(HabitDrinkFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _habitService.ExportAllHabitsDrinkAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"HabitsDrink_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_HabitsDrink_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyHabitsDrugsZipAsync(HabitDrugFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _habitService.ExportAllHabitsDrugsAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"HabitsDrugs_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_HabitsDrugs_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyHabitsSleepZipAsync(HabitSleepFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _habitService.ExportAllHabitsSleepAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"HabitsSleep_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_HabitsSleep_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyMFUsHabitsZipAsync(PatientFilterDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString("yyyy-MM-dd");
                string csvFileName = "";

                byte[] csvBytes = await _habitService.ExportAllMFUsHabitsAsync(filter, cancellationToken);

                if (typeExport == "with_filter")
                {
                    csvFileName = $"MFUsHabits_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == "all")
                {
                    csvFileName = $"All_MFUsHabits_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, 0, csvBytes.Length, cancellationToken);
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
