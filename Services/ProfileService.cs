using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.Models;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services
{
    public class ProfileService : IProfile
    {
        private readonly AppDbContext _bd;
        private readonly ICalories _CaloriesService;

        public ProfileService(AppDbContext bd, ICalories CaloriesService)
        {
            _bd = bd;
            _CaloriesService = CaloriesService;
        }

        public async Task CreateProfileAsync(Guid accountID, AccountDto values, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var protocol = await _bd.Protocols.FirstOrDefaultAsync(e => e.protocolToFollow == values.protocolToFollow, cancellationToken);

            if (protocol is null) {

                protocol = new Protocols
                {
                    protocolToFollow = values.protocolToFollow
                };

                _bd.Protocols.Add(protocol);
            }

            Profiles profile = new Profiles
            {
                accountID = accountID,
                birthDate = values.birthDate,
                sex = values.sex,
                stature = values.stature,
                weight = values.weight,
                protocolID= protocol.protocolID,
                uiemID = values.uiemID
            };

            ValidationValuesDB.ValidationValues(profile);

            _bd.Profiles.Add(profile);

            if (!Save()) { throw new UnstoredValuesException(); }

            var userKcal = _CaloriesService.CreateUserCalories(profile);

            _CaloriesService.CreateCaloriesRequiredPerDays(userKcal, today);
        }

        public async Task<string> UpdateProfileAsync(ProfileDto values, CancellationToken cancellationToken)
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var profile = await _bd.Profiles.FirstOrDefaultAsync(e => e.accountID == values.accountID, cancellationToken);

            var protocol = await _bd.Protocols.FirstOrDefaultAsync(e => e.protocolToFollow == values.protocolToFollow, cancellationToken);

            if (protocol is null)
            {
                protocol = new Protocols
                {
                    protocolToFollow = values.protocolToFollow
                };

                _bd.Protocols.Add(protocol);
            }

            if (profile is null) { throw new UserNotFoundException(); }

            profile.sex = values.sex;
            profile.birthDate = values.birthDate;
            profile.stature = values.stature;
            profile.weight = values.weight;
            profile.protocolID = protocol.protocolID;
            profile.uiemID = values.uiemID;

            ValidationValuesDB.ValidationValues(profile);

            if (!Save()) { throw new UnstoredValuesException(); }

            var userKcal = await _CaloriesService.UpdateUserCaloriesAsync(profile, cancellationToken);

            await _CaloriesService.CaloriesRequiredPerDaysAsync(values.accountID, today, cancellationToken);

            return "Su cuenta se actualiz&oacute; con &eacute;xito.";
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
