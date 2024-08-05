using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;

namespace AppVidaSana.Services.IServices.IHabits.IHabits
{
    public interface IDrinkHabit
    {
        GetDrinksConsumedDto AddDrinksConsumed(DrinksConsumedDto drinksConsumed);

        GetDrinksConsumedDto UpdateDrinksConsumed(UpdateDrinksConsumedDto values);

        string DeleteDrinksConsumed(Guid idHabit);

        bool Save();

    }
}
