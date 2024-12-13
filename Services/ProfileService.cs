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

            CreateUserCalories(profile);

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

            ValidationValuesDB.ValidationValues(profile);

            await UpdateUserCalories(values, cancellationToken);

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

        private void CreateUserCalories(Profiles profile)
        {
            float kcalNeeded = 0;

            int age = GetAge(profile.birthDate);

            if (profile.sex.Equals("Masculino"))
            {
                kcalNeeded = 88.362f + (13.397f * profile.weight) +(4.799f * profile.stature) - (5.677f * age);
            }

            if (profile.sex.Equals("Femenino"))
            {
                kcalNeeded = 447.593f + (9.247f * profile.weight) +(3.098f * profile.stature) - (4.330f * age);
            }

            UserCalories userKcal = new UserCalories
            {
                accountID = profile.accountID,
                caloriesNeeded = kcalNeeded
            };

            ValidationValuesDB.ValidationValues(profile);

            _bd.UserCalories.Add(userKcal);
        }

        private static int GetAge(DateOnly date)
        {
            DateTime dateActual = DateTime.Today;
            int age = dateActual.Year - date.Year;

            if (date.Month > dateActual.Month || (date.Month == dateActual.Month && date.Day > dateActual.Day))
            {
                age--;
            }

            return age;
        }
    
        private async Task UpdateUserCalories(ProfileDto profile, CancellationToken cancellationToken)
        {
            var userKcal = await _bd.UserCalories.FirstOrDefaultAsync(e => e.accountID == profile.accountID, cancellationToken);

            float kcalNeeded = 0;

            int age = GetAge(profile.birthDate);

            if (profile.sex.Equals("Masculino"))
            {
                kcalNeeded = 88.362f + (13.397f * profile.weight) + (4.799f * profile.stature) - (5.677f * age);
            }

            if (profile.sex.Equals("Femenino"))
            {
                kcalNeeded = 447.593f + (9.247f * profile.weight) + (3.098f * profile.stature) - (4.330f * age);
            }

            userKcal!.caloriesNeeded = kcalNeeded;

            ValidationValuesDB.ValidationValues(userKcal);
        }
    }
}
