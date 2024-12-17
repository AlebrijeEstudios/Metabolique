using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Feeding;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services
{
    public class ProfileService : IProfile
    {
        private readonly AppDbContext _bd;

        public ProfileService(AppDbContext bd)
        {
            _bd = bd;
        }

        public void CreateProfile(Guid accountID, AccountDto values)
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

            ValidationValuesDB.ValidationValues(profile);

            _bd.Profiles.Add(profile);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        public async Task<string> UpdateProfileAsync(ProfileDto values, CancellationToken cancellationToken)
        {
            var profile = await _bd.Profiles.FindAsync(new object[] { values.accountID }, cancellationToken);

            if (profile is null) { throw new UserNotFoundException(); }

            profile.sex = values.sex;
            profile.birthDate = values.birthDate;
            profile.stature = values.stature;
            profile.weight = values.weight;
            profile.protocolToFollow = values.protocolToFollow;

            ValidationValuesDB.ValidationValues(profile);;

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
