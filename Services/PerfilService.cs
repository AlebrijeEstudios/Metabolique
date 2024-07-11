using AppVidaSana.Data;
using AppVidaSana.Models;
using AppVidaSana.Services.IServices;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AutoMapper;
using AppVidaSana.Exceptions;
using System.ComponentModel.DataAnnotations;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;

namespace AppVidaSana.Services
{
    public class PerfilService : IPerfil
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public PerfilService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;

        }

        public string CreateProfile(Guid id, ProfileUserDto profile)
        {
            var c = _bd.Cuentas.Find(id);

            if (c == null)
            {
                throw new UserNotFoundException();
            }

            Perfil us = new Perfil
            {
                id = id,
                fechanacimiento = profile.fechaNacimiento,
                sexo = profile.sexo,
                estatura = profile.estatura,
                peso = profile.peso,
                protocolo = profile.protocolo,
                cuenta = null
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(us, null, null);

            if (!Validator.TryValidateObject(us, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();
                throw new ErrorDatabaseException(errors);
            }

            _bd.Perfiles.Add(us);

            Guardar();
            return "Cambios guardados";

        }

        public ProfileUserDto GetProfile(Guid userid)
        {
            var user = _bd.Perfiles.Find(userid);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            ProfileUserDto profile = _mapper.Map<ProfileUserDto>(user);

            return profile;
        }


        public string UpdateProfile(Guid id, ProfileUserDto profiledto)
        {
            var up = _bd.Perfiles.Find(id);

            if (up == null)
            {
                throw new UserNotFoundException();
            }

            up.sexo = profiledto.sexo;
            up.fechanacimiento = profiledto.fechaNacimiento;
            up.estatura = profiledto.estatura;
            up.peso = profiledto.peso;
            up.protocolo = profiledto.protocolo;

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(up, null, null);

            if (!Validator.TryValidateObject(up, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();
                throw new ErrorDatabaseException(errors);
            }

            _bd.Perfiles.Update(up);
            Guardar();
            return "Actualización completada";
        }

        public bool Guardar()
        {
            try
            {
                return _bd.SaveChanges() >= 0;
            }catch(Exception)
            {
                return false;

            }
            
        }
    }
}
