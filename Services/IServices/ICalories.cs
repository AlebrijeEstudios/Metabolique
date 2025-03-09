using AppVidaSana.Models.Feeding;
using AppVidaSana.Models;

namespace AppVidaSana.Services.IServices
{
    public interface ICalories
    {
        UserCalories CreateUserCalories(Profiles profile);

        void CreateCaloriesRequiredPerDays(UserCalories userKcal, DateOnly date);

        Task<UserCalories> UpdateUserCaloriesAsync(Profiles profile, CancellationToken cancellationToken);

        Task UpdateCaloriesRequiredPerDaysAsync(UserCalories userKcal, DateOnly date, CancellationToken cancellationToken);

        bool Save();
    }
}
