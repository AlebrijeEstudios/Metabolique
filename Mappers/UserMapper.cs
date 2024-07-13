using AutoMapper;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;

namespace AppVidaSana.Mappers
{
    public class UserMapper : Profile
    {
        public UserMapper()
        {
            CreateMap<Cuenta, UserInfoDto>().ReverseMap();
            CreateMap<Perfil, ProfileUserDto>().ReverseMap();
            CreateMap<Ejercicio, ListaEjerciciosDto>().ReverseMap();
            CreateMap<SegMenEjercicio, RecuperarRespuestasDto>().ReverseMap();
        }
    }
}
