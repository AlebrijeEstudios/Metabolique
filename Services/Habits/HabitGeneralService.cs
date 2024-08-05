using AppVidaSana.Data;
using AppVidaSana.Mappers;
using AppVidaSana.Models.Dtos.Habits_Dtos;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drugs;
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
            var habitsDrink = _bd.habitsDrink.Where(e => e.accountID == idAccount && e.drinkDateHabit == date).ToList();

            var habitSleep = _bd.habitsSleep.FirstOrDefault(e => e.accountID == idAccount && e.sleepDateHabit == date);

            var habitDrugs = _bd.habitsDrugs.FirstOrDefault(e => e.accountID == idAccount && e.drugsDateHabit == date);

            ReturnInfoHabitsDto info = new ReturnInfoHabitsDto
            {
                drinkConsumed = _mapper.Map<List<GetDrinksConsumedDto>>(habitsDrink),
                hoursSleep = _mapper.Map<GetSleepingHoursDto>(habitSleep),
                drugsConsumed = _mapper.Map<GetDrugsConsumedDto>(habitDrugs)
            };

            return info;
        }
    }
}
