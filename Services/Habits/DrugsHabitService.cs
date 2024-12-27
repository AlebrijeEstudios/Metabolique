using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drugs;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Services.IServices.IHabits.IHabits;
using AppVidaSana.ValidationValues;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;

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

        public DrugsHabitInfoDto AddDrugsConsumed(DrugsHabitDto values)
        {
            var habitDrugsExisting = _bd.HabitsDrugs.Any(e => e.accountID == values.accountID
                                                         && e.drugsDateHabit == values.dateRegister);

            if (habitDrugsExisting) { throw new RepeatRegistrationException(); }

            DrugsHabit drugHabit = new DrugsHabit
            {
                accountID = values.accountID,
                drugsDateHabit = values.dateRegister,
                cigarettesSmoked = values.cigarettesSmoked,
                predominantEmotionalState = values.predominantEmotionalState
            };

            ValidationValuesDB.ValidationValues(drugHabit);

            _bd.HabitsDrugs.Add(drugHabit);

            if (!Save()) { throw new UnstoredValuesException(); }

            var habitDrugs = _bd.HabitsDrugs.FirstOrDefault(e => e.accountID == values.accountID
                                                            && e.drugsDateHabit == values.dateRegister);

            var infoHabitsDrugs = _mapper.Map<DrugsHabitInfoDto>(habitDrugs);

            return infoHabitsDrugs;
        }

        public DrugsHabitInfoDto UpdateDrugsConsumed(Guid drugsHabitID, JsonPatchDocument values)
        {
            var habitDrugs = _bd.HabitsDrugs.Find(drugsHabitID);

            if (habitDrugs == null) { throw new HabitNotFoundException("No hay información de consumo de drogas. Inténtelo de nuevo."); }

            values.ApplyTo(habitDrugs);

            if (!Save()) { throw new UnstoredValuesException(); }

            var infoHabitsDrugs = _mapper.Map<DrugsHabitInfoDto>(habitDrugs);

            return infoHabitsDrugs;
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