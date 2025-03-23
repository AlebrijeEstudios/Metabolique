namespace AppVidaSana.Services.IServices.IAdminWeb
{
    public interface IAWExercise
    {
        Task<byte[]> ExportAllExercisesAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllActivesMinutesAsync(CancellationToken cancellationToken);

        Task<byte[]> ExportAllMFUsExerciseAsync(CancellationToken cancellationToken);
    }
}
