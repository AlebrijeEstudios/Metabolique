using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;

namespace AppVidaSana.Services.IServices.IHabits
{
    public interface IDrinkHabit
    {
        List<GetDrinksConsumedDto> GetDrinksConsumed(Guid idAccount, DateOnly date);

        List<GetDrinksConsumedDto> AddDrinksConsumed(DrinksConsumedDto drinksConsumed);

        List<GetDrinksConsumedDto> UpdateDrinksConsumed(UpdateDrinksConsumedDto values);

        List<GetDrinksConsumedDto> DeleteDrinksConsumed(Guid idHabit);

        bool Save();

    }
}
