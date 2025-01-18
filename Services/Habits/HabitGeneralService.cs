using AppVidaSana.Data;
using AppVidaSana.GraphicValues;
using AppVidaSana.Models.Dtos.Habits_Dtos;
using AppVidaSana.Models.Dtos.Habits_Dtos.ReturnInfoHabits;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Services.IServices.IHabits;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace AppVidaSana.Services.Habits
{
    public class HabitGeneralService : IHabitsGeneral
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public HabitGeneralService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
        }

        public async Task<ReturnInfoHabitsDto> GetInfoGeneralHabitsAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {
            int DayOfWeek = (int)date.DayOfWeek;

            DayOfWeek = DayOfWeek == 0 ? 7 : DayOfWeek;

            DateOnly dateInitial = date.AddDays(-(DayOfWeek - 1));
            DateOnly dateFinal = dateInitial.AddDays(6);

            var dates = DatesInRange.GetDatesInRange(dateInitial, dateFinal);

            var habitDrink = await _bd.HabitsDrink.FirstOrDefaultAsync(e => e.accountID == accountID 
                                                                       && e.drinkDateHabit == date, cancellationToken);

            var habitSleep = await _bd.HabitsSleep.FirstOrDefaultAsync(e => e.accountID == accountID 
                                                                       && e.sleepDateHabit == date, cancellationToken);

            var habitDrugs = await _bd.HabitsDrugs.FirstOrDefaultAsync(e => e.accountID == accountID 
                                                                       && e.drugsDateHabit == date, cancellationToken);

            var values = await _bd.HabitsSleep.Where(c => c.accountID == accountID
                                                     && dates.Contains(c.sleepDateHabit)).ToListAsync(cancellationToken);

            var hoursSleep = dates.Select(date => new GraphicValuesHabitSleepDto
                             {
                                date = date,
                                value = values.FirstOrDefault(e => e.sleepDateHabit == date)?.sleepHours ?? 0
                             }).ToList();

            CultureInfo ci = new CultureInfo("es-ES");

            var monthExist = await _bd.Months.FirstOrDefaultAsync(e => e.month == date.ToString("MMMM", ci)
                                                                  && e.year == Convert.ToInt32(date.ToString("yyyy")), 
                                                                  cancellationToken);

            if (monthExist is null)
            {
                return GetReturnInfoHabitsDto(habitDrink, habitSleep, habitDrugs, hoursSleep, false);
            }

            var mfuExist = await _bd.MFUsHabits.AnyAsync(e => e.accountID == accountID 
                                                         && e.monthID == monthExist.monthID, cancellationToken);

            if (!mfuExist)
            {
                return GetReturnInfoHabitsDto(habitDrink, habitSleep, habitDrugs, hoursSleep, false);
            }

            return GetReturnInfoHabitsDto(habitDrink, habitSleep, habitDrugs, hoursSleep, true);
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
    }
}
