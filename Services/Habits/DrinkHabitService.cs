using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Services.IServices.IHabits;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

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

        public string AddDrinksConsumed(DrinksConsumedDto drinksConsumed)
        {
            var user = _bd.Accounts.Find(drinksConsumed.accountID);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            DrinkHabit drinkHabit = new DrinkHabit
            {
                accountID = drinksConsumed.accountID,
                drinkDateHabit = drinksConsumed.drinkDateHabit,
                typeDrink = drinksConsumed.typeDrink,
                amountConsumed = drinksConsumed.amountConsumed,
                account = null
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(drinkHabit, null, null);

            if (!Validator.TryValidateObject(drinkHabit, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }
            _bd.habitsDrink.Add(drinkHabit);
            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return "Los datos han sido guardados correctamente.";

        }

        public List<GetDrinksConsumedDto> GetDrinksConsumed(Guid idAccount, DateOnly date)
        {
            var habits = _bd.habitsDrink
            .Where(e => e.accountID == idAccount && e.drinkDateHabit == date)
            .ToList();

            if (habits.Count == 0)
            {
                throw new HabitNotFoundException("No se encontraron registros de las bebidas consumidas este día, inténtelo de nuevo");
            }

            var habitsDrink = _mapper.Map<List<GetDrinksConsumedDto>>(habits);

            return habitsDrink;
        }

        public string UpdateDrinksConsumed(UpdateDrinksConsumedDto values)
        {
            var habit = _bd.habitsDrink.Find(values.drinkHabitID);

            if (habit == null)
            {
                throw new HabitNotFoundException();
            }

            habit.typeDrink = values.typeDrink;
            habit.amountConsumed = values.amountConsumed;

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(habit, null, null);

            if (!Validator.TryValidateObject(habit, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }

            _bd.habitsDrink.Update(habit);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return "Actualización completada.";
        }

        public string DeleteDrinksConsumed(Guid idHabit)
        {
            var habit = _bd.habitsDrink.Find(idHabit);

            if (habit == null)
            {
                throw new HabitNotFoundException();
            }

            _bd.habitsDrink.Remove(habit);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return "Se ha eliminado correctamente.";
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
