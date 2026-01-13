using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
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

        private const string exportFilter = "with_filter";
        private const string exportAll = "all";
        private const string formatDate = "yyyy-MM-dd";

        public AWExportToZipService(IAWPatients patientsService, IAWFeeding feedingService, IAWMedication medicationService, IAWExercise exerciseService, IAWHabits habitService)
        {
            _patientsService = patientsService;
            _feedingService = feedingService;
            _medicationService = medicationService;
            _exerciseService = exerciseService;
            _habitService = habitService;
        }

        /*Patients*/
        public async Task<byte[]> GenerateOnlyPatientsZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _patientsService.ExportPatientsAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"Patients_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_Patients_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        /*Feeding*/
        public async Task<byte[]> GenerateOnlyFeedingsZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _feedingService.ExportAllFeedingsAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"InfoFeedings_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_InfoFeedings_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyFoodsConsumedPerFeedingZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _feedingService.ExportAllFoodsConsumedPerFeedingAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"FoodsConsumedPerFeedingPerPatient_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_FoodsConsumedPerFeedingPerPatient_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyUserCaloriesZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _feedingService.ExportAllUserCaloriesAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"CaloriesRequiredPerPatient_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_CaloriesRequiredPerPatient_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyMFUsFeedingZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _feedingService.ExportAllMFUsFeedingAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"MFUsFeeding_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_MFUsFeeding_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyCaloriesConsumedZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _feedingService.ExportAllCaloriesConsumedAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"TotalCaloriesConsumedPerPatientPerDay_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_TotalCaloriesConsumedPerPatientPerDay_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyCaloriesRequiredPerDaysZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _feedingService.ExportAllCaloriesRequiredPerDaysAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"CaloriesRequiredPerDaysPerPatient_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_CaloriesRequiredPerDaysPerPatient_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        /*Medication*/
        public async Task<byte[]> GenerateOnlyPeriodMedicationsZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _medicationService.ExportAllPeriodMedicationsAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"PeriodsMedications_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_PeriodsMedications_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlySideEffectsZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _medicationService.ExportAllSideEffectsAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"SideEffects_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_SideEffects_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyMFUsMedicationZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _medicationService.ExportAllMFUsMedicationAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"MFUsMedication_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_MFUsMedication_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        /*Exercise*/
        public async Task<byte[]> GenerateOnlyExercisesZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _exerciseService.ExportAllExercisesAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"Exercises_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_Exercises_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyMFUsExerciseZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _exerciseService.ExportAllMFUsExerciseAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"MFUsExercise_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_MFUsExercise_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyActivesMinutesZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _exerciseService.ExportAllActivesMinutesAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"ActivesMinutes_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_ActivesMinutes_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        /*Habit*/
        public async Task<byte[]> GenerateOnlyHabitsDrinkZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _habitService.ExportAllHabitsDrinkAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"HabitsDrink_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_HabitsDrink_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyHabitsDrugsZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _habitService.ExportAllHabitsDrugsAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"HabitsDrugs_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_HabitsDrugs_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyHabitsSleepZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _habitService.ExportAllHabitsSleepAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"HabitsSleep_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_HabitsSleep_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }

        public async Task<byte[]> GenerateOnlyMFUsHabitsZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken)
        {
            using var mainMemoryStream = new MemoryStream();

            using (var mainZip = new ZipArchive(mainMemoryStream, ZipArchiveMode.Create, true))
            {
                string dateSuffix = DateTime.Today.ToString(formatDate);
                string csvFileName = "";

                byte[] csvBytes = await _habitService.ExportAllMFUsHabitsAsync(filter, cancellationToken);

                if (typeExport == exportFilter)
                {
                    csvFileName = $"MFUsHabits_With_Filters_{dateSuffix}.csv";
                }

                if (typeExport == exportAll)
                {
                    csvFileName = $"All_MFUsHabits_{dateSuffix}.csv";
                }

                var entry = mainZip.CreateEntry(csvFileName, CompressionLevel.Optimal);
                using var entryStream = entry.Open();
                await entryStream.WriteAsync(csvBytes, cancellationToken);
            }

            return mainMemoryStream.ToArray();
        }
    }
}
