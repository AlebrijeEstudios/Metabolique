using AppVidaSana.Models.Dtos.Habits_Dtos.Sleep;
using Microsoft.AspNetCore.JsonPatch;

namespace AppVidaSana.Services.IServices.IHabits
{
    public interface ISleepHabit
    {
        Task<SleepHabitInfoDto> AddSleepHoursAsync(SleepHabitDto values, CancellationToken cancellationToken);

        Task<SleepHabitInfoDto> UpdateSleepHoursAsync(Guid sleepHabitID, JsonPatchDocument values, CancellationToken cancellationToken);
    }
}
