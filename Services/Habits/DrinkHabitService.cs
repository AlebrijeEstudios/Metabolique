﻿using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.Models.Habits;
using AppVidaSana.Services.IServices.IHabits;
using AppVidaSana.ValidationValues;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;

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

        public async Task<DrinkHabitInfoDto> AddDrinksConsumedAsync(DrinkHabitDto values, CancellationToken cancellationToken)
        {
            var habitDrinkExist = await _bd.HabitsDrink.AnyAsync(e => e.accountID == values.accountID
                                                                 && e.drinkDateHabit == values.dateRegister, cancellationToken);

            if (habitDrinkExist) { throw new RepeatRegistrationException(); }

            DrinkHabit drinkHabit = new DrinkHabit
            {
                accountID = values.accountID,
                drinkDateHabit = values.dateRegister,
                amountConsumed = values.amountConsumed
            };

            ValidationValuesDB.ValidationValues(drinkHabit);

            await _bd.HabitsDrink.AddAsync(drinkHabit, cancellationToken);

            try
            {
                await _bd.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {

                throw new UnstoredValuesException();

            }

            var infoHabitsDrink = _mapper.Map<DrinkHabitInfoDto>(drinkHabit);

            return infoHabitsDrink;
        }

        public async Task<DrinkHabitInfoDto> UpdateDrinksConsumedAsync(Guid drinkHabitID, JsonPatchDocument values, CancellationToken cancellationToken)
        {
            var habitDrink = await _bd.HabitsDrink.FindAsync(new object[] { drinkHabitID }, cancellationToken);

            if (habitDrink is null) { throw new HabitNotFoundException("No hay informaci&oacute;n de la cantidad consumida. Int&eacute;ntelo de nuevo."); }

            values.ApplyTo(habitDrink);

            try
            {
                await _bd.SaveChangesAsync(cancellationToken);
            }
            catch (Exception)
            {

                throw new UnstoredValuesException();

            }

            var infoHabitsDrink = _mapper.Map<DrinkHabitInfoDto>(habitDrink);

            return infoHabitsDrink;
        }
    }
}
