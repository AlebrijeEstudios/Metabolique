using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Graphics_Dtos;
using AppVidaSana.Models.Dtos.Habits_Dtos;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drugs;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;
using AppVidaSana.Models.Graphics;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AutoMapper;


namespace AppVidaSana.Mappers
{
    public class Mapper : AutoMapper.Profile
    {
        public Mapper()
        {
            CreateMap<Account, AccountInfoDto>().ReverseMap();
            CreateMap<Profiles, ProfileUserDto>().ReverseMap();
            CreateMap<Exercise, ExerciseListDto>().ReverseMap();
            CreateMap<MFUsExercise, RetrieveResponsesExerciseDto>().ReverseMap();
            CreateMap<GExercise, GExerciseDto>().ReverseMap();
            CreateMap<DrinkHabit, GetDrinksConsumedDto>().ReverseMap();
            CreateMap<SleepHabit, GetSleepingHoursDto>().ReverseMap();
            CreateMap<DrugsHabit, GetDrugsConsumedDto>().ReverseMap();
        }
    }
}
