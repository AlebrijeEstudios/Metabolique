using AppVidaSana.Data;
using AppVidaSana.Months_Dates;
using AppVidaSana.Models.Dtos.Habits_Dtos;
using AppVidaSana.Models.Dtos.Habits_Dtos.ReturnInfoHabits;
using AppVidaSana.Models.Habits;
using AppVidaSana.Services.IServices.IHabits;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using AppVidaSana.Services.IServices;

namespace AppVidaSana.Services.Habits
{
    public class HabitGeneralService : IHabitsGeneral
    {
        private readonly IDbContextFactory<AppDbContext> _contextFactory;
        private readonly IMapper _mapper;
        private readonly ICalories _CaloriesService;

        public HabitGeneralService(IDbContextFactory<AppDbContext> contextFactory, IMapper mapper, ICalories CaloriesService)
        {
            _contextFactory = contextFactory;
            _mapper = mapper;
            _CaloriesService = CaloriesService;
        }

        public async Task<ReturnInfoHabitsDto> GetInfoGeneralHabitsAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {
            await _CaloriesService.CaloriesRequiredPerDaysAsync(accountID, date, cancellationToken);

            using var context = _contextFactory.CreateDbContext();

            int DayOfWeek = (int)date.DayOfWeek;

            DayOfWeek = DayOfWeek == 0 ? 7 : DayOfWeek;

            DateOnly dateInitial = date.AddDays(-(DayOfWeek - 1));
            DateOnly dateFinal = dateInitial.AddDays(6);

            var dates = DatesInRange.GetDatesInRange(dateInitial, dateFinal);

            var habitDrink = GetDrinkHabitAsync(accountID, date, cancellationToken);

            var habitSleep = GetSleepHabitAsync(accountID, date, cancellationToken);

            var habitDrugs = GetDrugsHabitAsync(accountID, date, cancellationToken);

            var hoursSleep = GetGraphicValuesHabitSleepAsync(accountID, dates, cancellationToken); 

            await Task.WhenAll(habitDrink, habitSleep, habitDrugs, hoursSleep);

            var habitDrinkResult =  habitDrink.Result;

            var habitSleepResult =  habitSleep.Result;

            var habitDrugsResult =  habitDrugs.Result;

            var hoursSleepResult =  hoursSleep.Result;

            CultureInfo ci = new CultureInfo("es-ES");

            var monthExist = await context.Months.FirstOrDefaultAsync(e => e.month == date.ToString("MMMM", ci)
                                                                      && e.year == Convert.ToInt32(date.ToString("yyyy")), 
                                                                      cancellationToken);

            if (monthExist is null)
            {
                return GetReturnInfoHabitsDto(habitDrinkResult, habitSleepResult, habitDrugsResult, hoursSleepResult, false);
            }

            var mfuExist = await context.MFUsHabits.AnyAsync(e => e.accountID == accountID 
                                                             && e.monthID == monthExist.monthID, cancellationToken);

            if (!mfuExist)
            {
                return GetReturnInfoHabitsDto(habitDrinkResult, habitSleepResult, habitDrugsResult, hoursSleepResult, false);
            }

            return GetReturnInfoHabitsDto(habitDrinkResult, habitSleepResult, habitDrugsResult, hoursSleepResult, true);
        }

        private ReturnInfoHabitsDto GetReturnInfoHabitsDto(DrinkHabit? habitDrink, SleepHabit? habitSleep,
                                                           DrugsHabit? habitDrugs, List<GraphicValuesHabitSleepDto> hoursSleep,
                                                           bool status)
        {
            ReturnInfoHabitsDto infoHabits = new ReturnInfoHabitsDto
            {
                drinkConsumed = _mapper.Map<GetDrinkConsumedDto>(habitDrink),
                hoursSleepConsumed = _mapper.Map<GetHoursSleepConsumedDto>(habitSleep),
                drugsConsumed = _mapper.Map<GetDrugsConsumedDto>(habitDrugs),
                hoursSleep = hoursSleep,
                mfuStatus = status
            };

            return infoHabits;
        }

        private async Task<DrinkHabit?> GetDrinkHabitAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken) 
        {
            using var context = _contextFactory.CreateDbContext();

            var habitDrink = await context.HabitsDrink.FirstOrDefaultAsync(e => e.accountID == accountID
                                                                           && e.drinkDateHabit == date, cancellationToken);

            return habitDrink;
        }

        private async Task<DrugsHabit?> GetDrugsHabitAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext();

            var habitDrugs = await context.HabitsDrugs.FirstOrDefaultAsync(e => e.accountID == accountID
                                                                           && e.drugsDateHabit == date, cancellationToken);

            return habitDrugs;
        }

        private async Task<SleepHabit?> GetSleepHabitAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext();

            var habitSleep = await context.HabitsSleep.FirstOrDefaultAsync(e => e.accountID == accountID
                                                                           && e.sleepDateHabit == date, cancellationToken);

            return habitSleep;
        }

        private async Task<List<GraphicValuesHabitSleepDto>> GetGraphicValuesHabitSleepAsync(Guid accountID, List<DateOnly> dates, CancellationToken cancellationToken)
        {
            using var context = _contextFactory.CreateDbContext();

            var values = await context.HabitsSleep.Where(c => c.accountID == accountID
                                                         && dates.Contains(c.sleepDateHabit)).ToListAsync(cancellationToken);

            var hoursSleep = dates.Select(date => new GraphicValuesHabitSleepDto
            {
                date = date,
                value = values.FirstOrDefault(e => e.sleepDateHabit == date)?.sleepHours ?? 0
            }).ToList();

            return hoursSleep;
        }
    }
}
