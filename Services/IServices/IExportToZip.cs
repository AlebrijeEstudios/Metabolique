using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Exercise_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Feeding_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Habits_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Medication_AWDtos;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos.Patient_AWDtos;

namespace AppVidaSana.Services.IServices
{
    public interface IExportToZip
    {
        Task<byte[]> GenerateAllSectionsZipAsync(CancellationToken cancellationToken);

        Task<byte[]> GenerateOnlyPatientsZipAsync(PatientFilterDto? filter, string typeExport, CancellationToken cancellationToken);

        Task<byte[]> GenerateOnlyFeedingsZipAsync(UserFeedFilterDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyFoodsConsumedPerFeedingZipAsync(UserFeedFilterDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyCaloriesConsumedZipAsync(CaloriesConsumedFilterDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyCaloriesRequiredPerDaysZipAsync(CaloriesRequiredPerDaysFilterDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyUserCaloriesZipAsync(PatientFilterDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyMFUsFeedingZipAsync(PatientFilterDto? filter, string typeExport, CancellationToken cancellationToken);


        Task<byte[]> GenerateOnlyPeriodMedicationsZipAsync(PeriodMedicationsFilterDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlySideEffectsZipAsync(SideEffectsFilterDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyMFUsMedicationZipAsync(MFUsMedicationFilterDto? filter, string typeExport, CancellationToken cancellationToken);


        Task<byte[]> GenerateOnlyExercisesZipAsync(ExerciseFilterDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyActivesMinutesZipAsync(ActiveMinutesFilterDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyMFUsExerciseZipAsync(PatientFilterDto? filter, string typeExport, CancellationToken cancellationToken);


        Task<byte[]> GenerateOnlyHabitsDrinkZipAsync(HabitDrinkFilterDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyHabitsDrugsZipAsync(HabitDrugFilterDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyHabitsSleepZipAsync(HabitSleepFilterDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyMFUsHabitsZipAsync(PatientFilterDto? filter, string typeExport, CancellationToken cancellationToken);
    }
}
