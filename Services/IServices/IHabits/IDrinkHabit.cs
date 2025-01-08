using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using Microsoft.AspNetCore.JsonPatch;

namespace AppVidaSana.Services.IServices.IHabits.IHabits
{
    public interface IDrinkHabit
    {
        DrinkHabitInfoDto AddDrinksConsumed(DrinkHabitDto values);

        DrinkHabitInfoDto UpdateDrinksConsumed(Guid sleepHabitID, JsonPatchDocument values); 

        bool Save();
    }
}
