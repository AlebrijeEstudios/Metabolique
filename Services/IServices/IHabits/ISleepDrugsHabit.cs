using AppVidaSana.Models.Dtos.Habits_Dtos;
using AppVidaSana.Models.Dtos.Habits_Dtos.Sleep_And_Drugs;

namespace AppVidaSana.Services.IServices.IHabits.IHabits
{
    public interface ISleepDrugsHabit
    {
        ReturnSleepHoursAndDrugsConsumedDto AddSleepHoursAndDrugsConsumed(SleepingHoursAndDrugsConsumedDto values);

        ReturnSleepHoursAndDrugsConsumedDto UpdateSleepHoursAndDrugsConsumed(ReturnSleepHoursAndDrugsConsumedDto values);

        string DeleteSleepHours(Guid idHabit);

        string DeleteDrugsConsumed(Guid idHabit);

        bool Save();

    }
}
