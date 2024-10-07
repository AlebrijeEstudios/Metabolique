using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;

namespace AppVidaSana.Services
{
    public class ProfileService : IProfile
    {
        private readonly AppDbContext _bd;
        private ValidationValuesDB _validationValues;

        public ProfileService(AppDbContext bd)
        {
            _bd = bd;
            _validationValues = new ValidationValuesDB();
        }

        public async void CreateProfile(Guid accountID, AccountDto values)
        {
            Profiles profile = new Profiles
            {
                accountID = accountID,
                birthDate = values.birthDate,
                sex = values.sex,
                stature = values.stature,
                weight = values.weight,
                protocolToFollow = values.protocolToFollow
            };

            _validationValues.ValidationValues(profile);

            await _bd.Profiles.AddAsync(profile);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        public async Task<string> UpdateProfile(ProfileDto values)
        {
            var profile = await _bd.Profiles.FindAsync(values.accountID);

            if (profile == null) { throw new UserNotFoundException(); }

            profile.sex = values.sex;
            profile.birthDate = profile.birthDate;
            profile.stature = values.stature;
            profile.weight = values.weight;
            profile.protocolToFollow = values.protocolToFollow;

            _validationValues.ValidationValues(profile);

            _bd.Profiles.Update(profile);

            if (!Save()) { throw new UnstoredValuesException(); }

            return "Su cuenta se actualizó con éxito.";
        }

        public bool Save()
        {
            try
            {
                return _bd.SaveChanges() >= 0;

            }
            catch (Exception)
            {
                return false;

            }
        }
    }
}
