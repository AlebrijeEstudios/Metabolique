using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Habits_Dtos;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Services.IServices.IHabits;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Services.Habits
{
    public class SleepHabitService : ISleepHabit
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public SleepHabitService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
        }

        public string AddSleepHours(SleepingHoursDto sleepingHours)
        {
            var user = _bd.Accounts.Find(sleepingHours.accountID);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            SleepHabit sleepHabit = new SleepHabit
            {
                accountID = sleepingHours.accountID,
                sleepDateHabit = sleepingHours.sleepDateHabit,
                sleepHours = sleepingHours.sleepHours,
                perceptionOfRelaxation = sleepingHours.perceptionOfRelaxation,
                account = null
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(sleepingHours, null, null);

            if (!Validator.TryValidateObject(sleepingHours, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }
            _bd.habitsSleep.Add(sleepHabit);
            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return "Los datos han sido guardados correctamente.";
        }

        public List<GetSleepingHoursDto> GetSleepingHours(Guid idAccount, DateOnly date)
        {
            var habits = _bd.habitsSleep
            .Where(e => e.accountID == idAccount && e.sleepDateHabit == date)
            .ToList();

            if (habits.Count == 0)
            {
                throw new HoursSleepNotFoundException();
            }

            var habitsSleep = _mapper.Map<List<GetSleepingHoursDto>>(habits);

            return habitsSleep;
        }

        public string UpdateSleepHours(UpdateSleepingHoursDto values)
        {
            var habit = _bd.habitsSleep.Find(values.sleepHabitID);

            if (habit == null)
            {
                throw new HabitNotFoundException();
            }

            habit.sleepHours = values.sleepHours;
            habit.perceptionOfRelaxation = values.perceptionOfRelaxation;

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

            _bd.habitsSleep.Update(habit);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return "Actualización completada.";
        } 
        
        public string DeleteSleepHours(Guid idHabit)
        {
            var habit = _bd.habitsSleep.Find(idHabit);

            if (habit == null)
            {
                throw new HabitNotFoundException();
            }

            _bd.habitsSleep.Remove(habit);

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
