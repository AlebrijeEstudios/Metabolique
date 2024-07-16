using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;
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
            CreateMap<MFUsExcercise, RetrieveResponsesDto>().ReverseMap();
        }
    }
}
