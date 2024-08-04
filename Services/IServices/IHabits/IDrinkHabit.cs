using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;

namespace AppVidaSana.Services.IServices.IHabits.IHabits
{
    public interface IDrinkHabit
    {
        List<GetDrinksConsumedDto> AddDrinksConsumed(DrinksConsumedDto drinksConsumed);

        List<GetDrinksConsumedDto> UpdateDrinksConsumed(UpdateDrinksConsumedDto values);

        List<GetDrinksConsumedDto> DeleteDrinksConsumed(Guid idHabit);

        bool Save();

    }
}
