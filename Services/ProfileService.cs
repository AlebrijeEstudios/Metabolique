using AppVidaSana.Data;
using AppVidaSana.Models;
using AppVidaSana.Services.IServices;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AutoMapper;
using AppVidaSana.Exceptions;
using System.ComponentModel.DataAnnotations;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Exceptions.Account_Profile;

namespace AppVidaSana.Services
{
    public class ProfileService : IProfile
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public ProfileService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
        }

        public bool CreateProfile(Guid id, CreateAccountProfileDto profile)
        {
            var account = _bd.Accounts.Find(id);

            if (account == null)
            {
                throw new UserNotFoundException();
            }

            Models.Profiles prf = new Models.Profiles
            {
                accountID = id,
                birthDate = profile.birthDate,
                sex = profile.sex,
                stature = profile.stature,
                weigth = profile.weigth,
                protocolToFollow  = profile.protocolToFollow,
                account = null
            };

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(prf, null, null);

            if (!Validator.TryValidateObject(prf, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }

            _bd.Profiles.Add(prf);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return Save();

        }

        public string UpdateProfile(Guid id, ProfileUserDto profile)
        {
            var prf = _bd.Profiles.Find(id);

            if (prf == null)
            {
                throw new UserNotFoundException();
            }

            prf.sex = profile.sex;
            prf.birthDate = profile.birthDate;
            prf.stature = profile.stature;
            prf.weigth = profile.weigth;
            prf.protocolToFollow = profile.protocolToFollow;

            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(prf, null, null);

            if (!Validator.TryValidateObject(prf, validationContext, validationResults, true))
            {
                var errors = validationResults.Select(vr => vr.ErrorMessage).ToList();

                if (errors.Count > 0)
                {
                    throw new ErrorDatabaseException(errors);
                }
            }

            _bd.Profiles.Update(prf);

            if (!Save())
            {
                throw new UnstoredValuesException();
            }

            return "Actualización completada";
        }

        public bool Save()
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
