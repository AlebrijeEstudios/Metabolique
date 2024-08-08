using AppVidaSana.Data;
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

            DateOnly dateFinal = date.AddDays(-6);

            var habits = _bd.HabitsSleep
                .Where(e => e.sleepDateHabit >= dateFinal && e.sleepDateHabit <= date && e.accountID == idAccount)
                .ToList();

            List<GetSleepingHoursDto> habitsSleep = _mapper.Map<List<GetSleepingHoursDto>>(habits);

            habitsSleep = habitsSleep.OrderBy(x => x.sleepDateHabit).ToList();


            ReturnInfoHabitsDto info = new ReturnInfoHabitsDto
            {
                drinkConsumed = _mapper.Map<List<GetDrinksConsumedDto>>(habitsDrink),
                hoursSleepConsumed = _mapper.Map<GetSleepingHoursDto>(habitSleep),
                drugsConsumed = _mapper.Map<GetDrugsConsumedDto>(habitDrugs),
                hoursSleep = habitsSleep
            };

            return info;
        }
    }
}
