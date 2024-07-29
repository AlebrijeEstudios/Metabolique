using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;

namespace AppVidaSana.Services.IServices.IHabits
{
    public interface IDrinkHabit
    {
        List<GetDrinksConsumedDto> GetDrinksConsumed(Guid idAccount, DateOnly date);

        string AddDrinksConsumed(DrinksConsumedDto drinksConsumed);

        string UpdateDrinksConsumed(UpdateDrinksConsumedDto values);

        string DeleteDrinksConsumed(Guid idHabit);

        bool Save();

    }
}
