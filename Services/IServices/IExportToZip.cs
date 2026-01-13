using AppVidaSana.Models.Dtos.AdminWeb_Dtos;

namespace AppVidaSana.Services.IServices
{
    public interface IExportToZip
    {
        Task<byte[]> GenerateOnlyPatientsZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);

        Task<byte[]> GenerateOnlyFeedingsZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyFoodsConsumedPerFeedingZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyUserCaloriesZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyMFUsFeedingZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);


        Task<byte[]> GenerateOnlyPeriodMedicationsZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlySideEffectsZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyMFUsMedicationZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);


        Task<byte[]> GenerateOnlyExercisesZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyMFUsExerciseZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);


        Task<byte[]> GenerateOnlyHabitsDrinkZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyHabitsDrugsZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyHabitsSleepZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyMFUsHabitsZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);

        Task<byte[]> GenerateOnlyCaloriesConsumedZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyCaloriesRequiredPerDaysZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);
        Task<byte[]> GenerateOnlyActivesMinutesZipAsync(FilterAdminDto? filter, string typeExport, CancellationToken cancellationToken);
    }
}
