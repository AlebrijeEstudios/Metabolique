using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Services.IServices.IHabits.IHabits;
using AutoMapper;
using System.Collections.Generic;
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

        public GetDrinksConsumedDto AddDrinksConsumed(DrinksConsumedDto drinksConsumed)
        {

            var habitExisting = _bd.HabitsDrink.Count(e => e.drinkDateHabit == drinksConsumed.drinkDateHabit && 
                                e.typeDrink == drinksConsumed.typeDrink &&
                                e.amountConsumed == drinksConsumed.amountConsumed);

            if (habitExisting > 0)
            {
                throw new RepeatRegistrationException();
            }

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
                amountConsumed = drinksConsumed.amountConsumed
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
            _bd.HabitsDrink.Add(drinkHabit);
            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            GetDrinksConsumedDto drinks = GetDrinksConsumed(drinksConsumed.accountID, drinksConsumed.drinkDateHabit,
                                                            drinksConsumed.typeDrink, drinksConsumed.amountConsumed);

            return drinks;

        } 

        public GetDrinksConsumedDto UpdateDrinksConsumed(UpdateDrinksConsumedDto values)
        {
            var habit = _bd.HabitsDrink.Find(values.drinkHabitID);

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

            _bd.HabitsDrink.Update(habit);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            GetDrinksConsumedDto drinks = GetDrinksConsumed(values.accountID, values.drinkDateHabit,
                                                            values.typeDrink, values.amountConsumed);

            return drinks;
        }

        public string DeleteDrinksConsumed(Guid idHabit)
        {
            var habit = _bd.HabitsDrink.Find(idHabit);

            if (habit == null)
            {
                throw new HabitNotFoundException();
            }

            Guid id = habit.accountID;
            DateOnly date = habit.drinkDateHabit;

            _bd.HabitsDrink.Remove(habit);

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
        
        private GetDrinksConsumedDto GetDrinksConsumed(Guid id, DateOnly date, string type, string amount)
        {
            var habits = _bd.HabitsDrink.FirstOrDefault(e => e.accountID == id && e.drinkDateHabit == date
                                                        && e.typeDrink == type &&
                                                        e.amountConsumed == amount);

            GetDrinksConsumedDto habitsDrink;

            if (habits == null)
            {
               habitsDrink = _mapper.Map<GetDrinksConsumedDto>(habits);  
            }

            habitsDrink = _mapper.Map<GetDrinksConsumedDto>(habits);

            return habitsDrink;
        }
    }
}
