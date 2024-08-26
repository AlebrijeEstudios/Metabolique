using AppVidaSana.Data;
using AppVidaSana.Exceptions.Medication;
using AppVidaSana.Mappers;
using AppVidaSana.Models.Dtos.Habits_Dtos;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.Models.Dtos.Habits_Dtos.Sleep_And_Drugs;
using AppVidaSana.Services.IServices.IHabits;
using AutoMapper;

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

        public ReturnInfoHabitsDto GetInfoGeneralHabits(Guid idAccount, DateOnly date)
        {
            var habitsDrink = _bd.HabitsDrink.Where(e => e.accountID == idAccount && e.drinkDateHabit == date).ToList();

            var habitSleep = _bd.HabitsSleep.FirstOrDefault(e => e.accountID == idAccount && e.sleepDateHabit == date);

            var habitDrugs = _bd.HabitsDrugs.FirstOrDefault(e => e.accountID == idAccount && e.drugsDateHabit == date);

            List<GraphicValuesHabitSleepDto> hoursSleep = new List<GraphicValuesHabitSleepDto>();
            
            DateOnly dateFinal = date.AddDays(-6);

            var dates = GetDatesInRange(dateFinal, date);

            foreach(var item in dates)
            {
                var habits = _bd.HabitsSleep.FirstOrDefault(e => e.sleepDateHabit == item
                                                            && e.accountID == idAccount);

                if(habits != null)
                {
                    GraphicValuesHabitSleepDto value = new GraphicValuesHabitSleepDto
                    {
                        date = item,
                        value = habits.sleepHours
                    };

                    hoursSleep.Add(value);
                }
                else
                {
                    GraphicValuesHabitSleepDto value = new GraphicValuesHabitSleepDto
                    {
                        date = item,
                        value = 0
                    };

                    hoursSleep.Add(value);
                }
            }

            ReturnInfoHabitsDto info = new ReturnInfoHabitsDto
            {
                drinkConsumed = _mapper.Map<List<GetDrinksConsumedDto>>(habitsDrink),
                hoursSleepConsumed = _mapper.Map<GetHoursSleepConsumedDto>(habitSleep),
                drugsConsumed = _mapper.Map<GetDrugsConsumedDto>(habitDrugs),
                hoursSleep = hoursSleep
            };

            return info;
        }

        private static List<DateOnly> GetDatesInRange(DateOnly startDate, DateOnly endDate)
        {
            List<DateOnly> dates = new List<DateOnly>();

            if (endDate >= startDate)
            {
                for (DateOnly date = startDate; date <= endDate; date = date.AddDays(1))
                {
                    dates.Add(date);
                }
            }

            return dates;
        }
    }
}
