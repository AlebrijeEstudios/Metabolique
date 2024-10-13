using AppVidaSana.Models.Dtos.Habits_Dtos.Sleep;
using Microsoft.AspNetCore.JsonPatch;

namespace AppVidaSana.Services.IServices.IHabits
{
    public interface ISleepHabit
    {
        SleepHabitInfoDto AddSleepHours(SleepHabitDto values);

        SleepHabitInfoDto UpdateSleepHours(Guid sleepHabitID, JsonPatchDocument values);

        bool Save();
    }
}
