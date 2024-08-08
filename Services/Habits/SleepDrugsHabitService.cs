using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Habits_Dtos;
using AppVidaSana.Models.Dtos.Habits_Dtos.Sleep_And_Drugs;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Services.IServices.IHabits.IHabits;
using AutoMapper;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Services.Habits
{
    public class SleepDrugsHabitService : ISleepDrugsHabit
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public SleepDrugsHabitService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
        }

        public ReturnSleepHoursAndDrugsConsumedDto AddSleepHoursAndDrugsConsumed(SleepingHoursAndDrugsConsumedDto values)
        {
            var habitSleepExisting = _bd.HabitsSleep.Any(e => e.sleepDateHabit == values.dateRegister);
            var habitDrugsExisting = _bd.HabitsDrugs.Any(e => e.drugsDateHabit == values.dateRegister);

            if (habitSleepExisting || habitDrugsExisting)
            {
                throw new RepeatRegistrationException();
            }

            var user = _bd.Accounts.Find(values.accountID);

            if (user == null)
            {
                throw new UserNotFoundException();
            }

            SleepHabit sleepHabit = new SleepHabit
            {
                accountID = values.accountID,
                sleepDateHabit = values.dateRegister,
                sleepHours = values.sleepHours,
                perceptionOfRelaxation = values.perceptionOfRelaxation
            };

            DrugsHabit drugHabit = new DrugsHabit
            {
                accountID = values.accountID,
                drugsDateHabit = values.dateRegister,
                cigarettesSmoked = values.cigarettesSmoked,
                predominantEmotionalState = values.predominantEmotionalState
            };

            ValidationResultsSleepHours(sleepHabit);
            ValidationResultsDrugsConsumed(drugHabit);

            _bd.HabitsSleep.Add(sleepHabit);
            _bd.HabitsDrugs.Add(drugHabit);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            ReturnSleepHoursAndDrugsConsumedDto habits;

            var habitSleep = _bd.HabitsSleep.FirstOrDefault(e => e.accountID == values.accountID
                                                            && e.sleepDateHabit == values.dateRegister);

            var habitDrug = _bd.HabitsDrugs.FirstOrDefault(e => e.accountID == values.accountID 
                                                            && e.drugsDateHabit == values.dateRegister);

            if (habitSleep == null || habitDrug == null)
            {
                return habits = new ReturnSleepHoursAndDrugsConsumedDto();
            }

            habits = _mapper.Map<ReturnSleepHoursAndDrugsConsumedDto>(habitSleep);
            habits = _mapper.Map(habitDrug, habits);

            return habits;
        }

        public ReturnSleepHoursAndDrugsConsumedDto UpdateSleepHoursAndDrugsConsumed(ReturnSleepHoursAndDrugsConsumedDto values)
        {
            var habitSleep = _bd.HabitsSleep.Find(values.sleepHabitID);
            var habitDrugs = _bd.HabitsDrugs.Find(values.drugsHabitID);

            if (habitSleep == null || habitDrugs == null)
            {
                throw new HabitNotFoundException();
            }

            habitSleep.sleepHours = values.sleepHours;
            habitSleep.perceptionOfRelaxation = values.perceptionOfRelaxation;

            habitDrugs.cigarettesSmoked = values.cigarettesSmoked;
            habitDrugs.predominantEmotionalState = values.predominantEmotionalState;

            ValidationResultsSleepHours(habitSleep);
            ValidationResultsDrugsConsumed(habitDrugs);

            _bd.HabitsSleep.Update(habitSleep);
            _bd.HabitsDrugs.Update(habitDrugs);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            ReturnSleepHoursAndDrugsConsumedDto habits;

            var sleep = _bd.HabitsSleep.Find(values.sleepHabitID);
            var drugs = _bd.HabitsDrugs.Find(values.drugsHabitID);

            if (sleep == null || drugs == null)
            {
                return habits = new ReturnSleepHoursAndDrugsConsumedDto();
            }

            habits = _mapper.Map<ReturnSleepHoursAndDrugsConsumedDto>(sleep);
            habits = _mapper.Map(drugs, habits);

            return habits;
        } 
        
        public string DeleteSleepHours(Guid idHabit)
        {
            var habit = _bd.HabitsSleep.Find(idHabit);

            if (habit == null)
            {
                throw new HabitNotFoundException();
            }

            _bd.HabitsSleep.Remove(habit);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return "Se ha eliminado correctamente.";
        }

        public string DeleteDrugsConsumed(Guid idHabit)
        {
            var habit = _bd.HabitsDrugs.Find(idHabit);

            if (habit == null)
            {
                throw new HabitNotFoundException();
            }

            _bd.HabitsDrugs.Remove(habit);

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

        private void ValidationResultsSleepHours(SleepHabit obj)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(obj, null, null);

            if (!Validator.TryValidateObject(obj, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }
        }

        private void ValidationResultsDrugsConsumed(DrugsHabit obj)
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(obj, null, null);

            if (!Validator.TryValidateObject(obj, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }
        }
    }
}