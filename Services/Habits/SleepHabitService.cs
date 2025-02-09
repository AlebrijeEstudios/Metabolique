﻿using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.Habits_Dtos.Sleep;
using AppVidaSana.Models.Habits;
using AppVidaSana.Services.IServices.IHabits;
using AutoMapper;
using AppVidaSana.ValidationValues;
using Microsoft.AspNetCore.JsonPatch;
using AppVidaSana.Exceptions.Habits;
using Microsoft.EntityFrameworkCore;

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

        public async Task<SleepHabitInfoDto> AddSleepHoursAsync(SleepHabitDto values, CancellationToken cancellationToken)
        {
            var habitSleepExist = await _bd.HabitsSleep.AnyAsync(e => e.accountID == values.accountID
                                                                 && e.sleepDateHabit == values.dateRegister, cancellationToken);

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

            var infoHabitsSleep = _mapper.Map<SleepHabitInfoDto>(sleepHabit);

            return infoHabitsSleep;
        }

        public async Task<SleepHabitInfoDto> UpdateSleepHoursAsync(Guid sleepHabitID, JsonPatchDocument values, CancellationToken cancellationToken)
        {
            var habitSleep = await _bd.HabitsSleep.FindAsync(new object[] { sleepHabitID }, cancellationToken);

            if (habitSleep is null) { throw new HabitNotFoundException("No hay información de horas de sueño. Inténtelo de nuevo."); }

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
