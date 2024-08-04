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

            List<GetDrinksConsumedDto> listDrinks;

            if (habitsDrink.Count == 0)
            {
                listDrinks = _mapper.Map<List<GetDrinksConsumedDto>>(habitsDrink);
            }

            listDrinks = _mapper.Map<List<GetDrinksConsumedDto>>(habitsDrink);

            ReturnInfoHabitsDto info = new ReturnInfoHabitsDto
            {
                drinkConsumed = listDrinks,
                sleepHabitID = habitSleep.sleepHabitID,
                sleepHours = habitSleep.sleepHours,
                perceptionOfRelaxation = habitSleep.perceptionOfRelaxation,
                drugsHabitID = habitDrugs.drugsHabitID,
                cigarettesSmoked = habitDrugs.cigarettesSmoked,
                predominantEmotionalState = habitDrugs.predominantEmotionalState
            };

            return info;
        }
    }
}
