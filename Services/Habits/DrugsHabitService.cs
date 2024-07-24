using AppVidaSana.Data;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drugs;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Services.IServices.IHabits;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Services.Habits
{
    public class DrugsHabitService : IDrugsHabit
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public DrugsHabitService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
        }

        public string AddDrugsConsumed(DrugsConsumedDto drugsConsumed)
        {
            var user = _bd.Accounts.Find(drugsConsumed.accountID);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            DrugsHabit drugHabit = new DrugsHabit
            {
                accountID = drugsConsumed.accountID,
                drugsDateHabit = drugsConsumed.drugsDateHabit,
                cigarettesSmoked = drugsConsumed.cigarettesSmoked,
                predominantEmotionalState = drugsConsumed.predominantEmotionalState,
                account = null
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(drugHabit, null, null);

            if (!Validator.TryValidateObject(drugHabit, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }
            _bd.habitsDrugs.Add(drugHabit);
            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return "Los datos han sido guardados correctamente.";
        }

        public GetDrugsConsumedDto GetDrugsConsumed(Guid idAccount, DateOnly date)
        {
            var habit = _bd.habitsDrugs
            .Where(e => e.accountID == idAccount && e.drugsDateHabit == date);

            if (habit == null)
            {
                throw new HabitNotFoundException();
            }

            var habitDrug = _mapper.Map<GetDrugsConsumedDto>(habit);

            return habitDrug;
        }

        public string UpdateDrugsConsumed(UpdateDrugsConsumedDto values)
        {
            var habit = _bd.habitsDrugs.Find(values.drugsHabitID);

            if (habit == null)
            {
                throw new HabitNotFoundException();
            }

            habit.cigarettesSmoked = values.cigarettesSmoked;
            habit.predominantEmotionalState = values.predominantEmotionalState;

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

            _bd.habitsDrugs.Update(habit);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return "Actualización completada.";
        }
        
        public string DeleteDrugsConsumed(Guid idHabit)
        {
            var habit = _bd.habitsDrugs.Find(idHabit);

            if (habit == null)
            {
                throw new HabitNotFoundException();
            }

            _bd.habitsDrugs.Remove(habit);

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
