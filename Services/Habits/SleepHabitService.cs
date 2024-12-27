using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.Habits_Dtos.Sleep;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Services.IServices.IHabits;
using AutoMapper;
using AppVidaSana.ValidationValues;
using Microsoft.AspNetCore.JsonPatch;
using AppVidaSana.Exceptions.Habits;

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

        public SleepHabitInfoDto AddSleepHours(SleepHabitDto values)
        {
            var habitSleepExist = _bd.HabitsSleep.Any(e => e.accountID == values.accountID
                                                      && e.sleepDateHabit == values.dateRegister);

            if (habitSleepExist) { throw new RepeatRegistrationException(); }

            SleepHabit sleepHabit = new SleepHabit
            {
                accountID = values.accountID,
                sleepDateHabit = values.dateRegister,
                sleepHours = values.sleepHours,
                perceptionOfRelaxation = values.perceptionOfRelaxation
            };

            ValidationValuesDB.ValidationValues(sleepHabit);

            _bd.HabitsSleep.Add(sleepHabit);

            if (!Save()) { throw new UnstoredValuesException(); }

            var habitSleep = _bd.HabitsSleep.FirstOrDefault(e => e.accountID == values.accountID
                                                            && e.sleepDateHabit == values.dateRegister);

            var infoHabitsSleep = _mapper.Map<SleepHabitInfoDto>(habitSleep);

            return infoHabitsSleep;
        }

        public SleepHabitInfoDto UpdateSleepHours(Guid sleepHabitID, JsonPatchDocument values)
        {
            var habitSleep = _bd.HabitsSleep.Find(sleepHabitID);

            if (habitSleep == null) { throw new HabitNotFoundException("No hay información de horas de sueño. Inténtelo de nuevo."); }

            values.ApplyTo(habitSleep);

            if (!Save()) { throw new UnstoredValuesException(); }

            var infoHabitsSleep = _mapper.Map<SleepHabitInfoDto>(habitSleep);

            return infoHabitsSleep;
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
