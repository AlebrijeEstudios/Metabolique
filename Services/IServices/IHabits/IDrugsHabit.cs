using AppVidaSana.Models.Dtos.Habits_Dtos.Drugs;

namespace AppVidaSana.Services.IServices.IHabits.IHabits
{
    public interface IDrugsHabit
    {
        GetDrugsConsumedDto AddDrugsConsumed(DrugsConsumedDto drugsConsumed);

        GetDrugsConsumedDto UpdateDrugsConsumed(UpdateDrugsConsumedDto values);

        string DeleteDrugsConsumed(Guid idHabit);

        bool Save();

    }
}
