using AppVidaSana.Models.Dtos.Habits_Dtos.Drugs;

namespace AppVidaSana.Services.IServices.IHabits
{
    public interface IDrugsHabit
    {
        GetDrugsConsumedDto GetDrugsConsumed(Guid idAccount, DateOnly date);

        string AddDrugsConsumed(DrugsConsumedDto drugsConsumed);

        string UpdateDrugsConsumed(UpdateDrugsConsumedDto values);

        string DeleteDrugsConsumed(Guid idHabit);

        bool Save();

    }
}
