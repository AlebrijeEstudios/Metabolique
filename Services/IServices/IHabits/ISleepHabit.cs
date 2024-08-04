using AppVidaSana.Models.Dtos.Habits_Dtos;

namespace AppVidaSana.Services.IServices.IHabits.IHabits
{
    public interface ISleepHabit
    {
        List<GetSleepingHoursDto> GetSleepingHours(Guid idAccount, DateOnly date);

        GetUpdateSleepingHoursDto AddSleepHours(SleepingHoursDto sleepingHours);

        GetUpdateSleepingHoursDto UpdateSleepHours(GetUpdateSleepingHoursDto values);

        string DeleteSleepHours(Guid idHabit);

        bool Save();

    }
}
