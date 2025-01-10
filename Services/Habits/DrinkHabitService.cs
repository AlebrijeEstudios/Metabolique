using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Services.IServices.IHabits.IHabits;
using AppVidaSana.ValidationValues;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;

namespace AppVidaSana.Services.Habits
{
    public class DrinkHabitService : IDrinkHabit
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public DrinkHabitService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
        }

        public DrinkHabitInfoDto AddDrinksConsumed(DrinkHabitDto values)
        {
            var habitDrinkExist = _bd.HabitsDrink.Any(e => e.accountID == values.accountID
                                                     && e.drinkDateHabit == values.dateRegister);

            if (habitDrinkExist) { throw new RepeatRegistrationException(); }

            DrinkHabit drinkHabit = new DrinkHabit
            {
                accountID = values.accountID,
                drinkDateHabit = values.dateRegister,
                amountConsumed = values.amountConsumed
            };

            ValidationValuesDB.ValidationValues(drinkHabit);

            _bd.HabitsDrink.Add(drinkHabit);

            if (!Save()) { throw new UnstoredValuesException(); }

            var infoHabitsDrink = _mapper.Map<DrinkHabitInfoDto>(drinkHabit);

            return infoHabitsDrink;
        }

        public DrinkHabitInfoDto UpdateDrinksConsumed(Guid drinkHabitID, JsonPatchDocument values)
        {
            var habitDrink = _bd.HabitsDrink.Find(drinkHabitID);

            if (habitDrink == null) { throw new HabitNotFoundException("No hay información de la cantidad consumida. Inténtelo de nuevo."); }

            values.ApplyTo(habitDrink);

            if (!Save()) { throw new UnstoredValuesException(); }

            var infoHabitsDrink = _mapper.Map<DrinkHabitInfoDto>(habitDrink);

            return infoHabitsDrink;
        }

        public bool Save()
        {
            try
            {
                return _bd.SaveChanges() >= 0;
            }
            catch (Exception)
            {
                return false;

            }
        }
    }
}
