using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using Microsoft.AspNetCore.JsonPatch;

namespace AppVidaSana.Services.IServices.IHabits
{
    public interface IDrinkHabit
    {
        Task<DrinkHabitInfoDto> AddDrinksConsumedAsync(DrinkHabitDto values, CancellationToken cancellationToken);

        Task<DrinkHabitInfoDto> UpdateDrinksConsumedAsync(Guid sleepHabitID, JsonPatchDocument values, CancellationToken cancellationTokens); 

        bool Save();
    }
}
