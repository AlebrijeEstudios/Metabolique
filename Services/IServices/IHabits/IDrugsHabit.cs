using AppVidaSana.Models.Dtos.Habits_Dtos.Drugs;
using Microsoft.AspNetCore.JsonPatch;

namespace AppVidaSana.Services.IServices.IHabits.IHabits
{
    public interface IDrugsHabit
    {
        DrugsHabitInfoDto AddDrugsConsumed(DrugsHabitDto values);

        DrugsHabitInfoDto UpdateDrugsConsumed(Guid drugsHabitID, JsonPatchDocument values);

        bool Save();

    }
}
