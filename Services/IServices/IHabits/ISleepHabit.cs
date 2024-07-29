using AppVidaSana.Models.Dtos.Habits_Dtos;

namespace AppVidaSana.Services.IServices.IHabits
{
    public interface ISleepHabit
    {
        List<GetSleepingHoursDto> GetSleepingHours(Guid idAccount, DateOnly date);

        string AddSleepHours(SleepingHoursDto sleepingHours);

        string UpdateSleepHours(UpdateSleepingHoursDto values);

        string DeleteSleepHours(Guid idHabit);

        bool Save();

    }
}
