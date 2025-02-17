using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drugs;
using AppVidaSana.Models.Habits;
using AppVidaSana.Services.IServices.IHabits;
using AppVidaSana.ValidationValues;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

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

        public async Task<DrugsHabitInfoDto> AddDrugsConsumedAsync(DrugsHabitDto values, CancellationToken cancellationToken)
        {
            var habitDrugsExisting = await _bd.HabitsDrugs.AnyAsync(e => e.accountID == values.accountID
                                                                    && e.drugsDateHabit == values.dateRegister, cancellationToken);

            if (habitDrugsExisting) { throw new RepeatRegistrationException(); }

            DrugsHabit drugHabit = new DrugsHabit
            {
                accountID = values.accountID,
                drugsDateHabit = values.dateRegister,
                cigarettesSmoked = values.cigarettesSmoked,
                predominantEmotionalState = values.predominantEmotionalState
            };

            ValidationValuesDB.ValidationValues(drugHabit);

            await _bd.HabitsDrugs.AddAsync(drugHabit, cancellationToken);

            try
            {
                await _bd.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {

                throw new UnstoredValuesException();

            }

            var infoHabitsDrugs = _mapper.Map<DrugsHabitInfoDto>(drugHabit);

            return infoHabitsDrugs;
        }

        public async Task<DrugsHabitInfoDto> UpdateDrugsConsumedAsync(Guid drugsHabitID, JsonPatchDocument values, CancellationToken cancellationToken)
        {
            var habitDrugs = await _bd.HabitsDrugs.FindAsync(new object[] { drugsHabitID }, cancellationToken);

            if (habitDrugs is null) { throw new HabitNotFoundException("No hay informaci&oacute;n de consumo de drogas. Int&eacute;ntelo de nuevo."); }

            values.ApplyTo(habitDrugs);

            try
            {
                await _bd.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {

                throw new UnstoredValuesException();

            }

            var infoHabitsDrugs = _mapper.Map<DrugsHabitInfoDto>(habitDrugs);

            return infoHabitsDrugs;
        }
    }
}