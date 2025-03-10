using AppVidaSana.Data;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Feeding;
using AppVidaSana.Models;
using AppVidaSana.Services.IServices;
using AppVidaSana.ValidationValues;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services
{
    public class CaloriesService : ICalories
    {
        private readonly AppDbContext _bd;

        public CaloriesService(AppDbContext bd)
        {
            _bd = bd;
        }

        public async Task CaloriesRequiredPerDaysAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {
            var userKcal = await _bd.UserCalories.FirstOrDefaultAsync(e => e.accountID == accountID, cancellationToken);

            var kcalRequiredPerDay = await _bd.CaloriesRequiredPerDays
                                              .AnyAsync(e => e.accountID == accountID
                                                            && e.dateInitial <= date
                                                            && date <= e.dateFinal, cancellationToken);

            if (!kcalRequiredPerDay)
            {
                CreateCaloriesRequiredPerDays(userKcal!, date);
            }
            else
            {
                await UpdateCaloriesRequiredPerDaysAsync(userKcal!, date, cancellationToken);
            }
        }

        public UserCalories CreateUserCalories(Profiles profile)
        {
            double kcalNeeded = 0;

            int age = GetAge(profile.birthDate);

            if (profile.sex.Equals("Masculino"))
            {
                kcalNeeded = 88.362 + (13.397 * profile.weight) + (4.799 * profile.stature) - (5.677 * age);
            }

            if (profile.sex.Equals("Femenino"))
            {
                kcalNeeded = 447.593 + (9.247 * profile.weight) + (3.098 * profile.stature) - (4.330 * age);
            }

            UserCalories userKcal = new UserCalories
            {
                accountID = profile.accountID,
                caloriesNeeded = kcalNeeded
            };

            ValidationValuesDB.ValidationValues(profile);

            _bd.UserCalories.Add(userKcal);

            if (!Save()) { throw new UnstoredValuesException(); }

            return userKcal;
        }

        public async Task<UserCalories> UpdateUserCaloriesAsync(Profiles profile, CancellationToken cancellationToken)
        {
            var userKcal = await _bd.UserCalories.FirstOrDefaultAsync(e => e.accountID == profile.accountID, cancellationToken);

            double kcalNeeded = 0;

            int age = GetAge(profile!.birthDate);

            if (profile.sex.Equals("Masculino"))
            {
                kcalNeeded = 88.362 + (13.397 * profile.weight) + (4.799 * profile.stature) - (5.677 * age);
            }

            if (profile.sex.Equals("Femenino"))
            {
                kcalNeeded = 447.593 + (9.247 * profile.weight) + (3.098 * profile.stature) - (4.330 * age);
            }

            userKcal!.caloriesNeeded = kcalNeeded;

            ValidationValuesDB.ValidationValues(userKcal);

            if (!Save()) { throw new UnstoredValuesException(); }

            return userKcal;
        }

        public void CreateCaloriesRequiredPerDays(UserCalories userKcal, DateOnly date)
        {
            int DayOfWeek = (int) date.DayOfWeek;

            DayOfWeek = DayOfWeek == 0 ? 7 : DayOfWeek;

            DateOnly dateInitial = date.AddDays(-(DayOfWeek - 1));
            DateOnly dateFinal = dateInitial.AddDays(6);

            CaloriesRequiredPerDay kcalRequiredPerDay = new CaloriesRequiredPerDay
            {
                accountID = userKcal.accountID,
                dateInitial = dateInitial,
                dateFinal = dateFinal,
                caloriesNeeded = userKcal!.caloriesNeeded
            };

            ValidationValuesDB.ValidationValues(kcalRequiredPerDay);

            _bd.CaloriesRequiredPerDays.Add(kcalRequiredPerDay);

            if (!Save()) { throw new UnstoredValuesException(); }
        }

        public async Task UpdateCaloriesRequiredPerDaysAsync(UserCalories userKcal, DateOnly date, CancellationToken cancellationToken)
        {
            var kcalRequiredPerDay = await _bd.CaloriesRequiredPerDays
                                              .FirstOrDefaultAsync(e => e.accountID == userKcal.accountID
                                                                   && e.dateInitial <= date
                                                                   && date <= e.dateFinal, cancellationToken);

            int daysForExercise = await _bd.ActiveMinutes.Where(e => e.accountID == userKcal.accountID
                                                                && kcalRequiredPerDay!.dateInitial <= e.dateExercise
                                                                && e.dateExercise <= kcalRequiredPerDay!.dateFinal)
                                                         .CountAsync(cancellationToken);

            if (daysForExercise != 0 && daysForExercise <= 3)
            {
                kcalRequiredPerDay!.caloriesNeeded = userKcal!.caloriesNeeded * 1.375;
            }

            if (3 < daysForExercise && daysForExercise <= 5)
            {
                kcalRequiredPerDay!.caloriesNeeded = userKcal!.caloriesNeeded * 1.55;
            }

            if (daysForExercise == 6 || daysForExercise == 7)
            {
                kcalRequiredPerDay!.caloriesNeeded = userKcal!.caloriesNeeded * 1.725;
            }

            int daysExtenuating = await _bd.Exercises.Where(e => e.accountID == userKcal.accountID
                                                            && kcalRequiredPerDay!.dateInitial <= e.dateExercise
                                                            && e.dateExercise <= kcalRequiredPerDay!.dateFinal
                                                            && e.intensityExercise == "Extenuante")
                                                     .Select(e => e.dateExercise)
                                                     .Distinct()
                                                     .CountAsync(cancellationToken);

            if (daysExtenuating == 6 || daysExtenuating == 7)
            {
                kcalRequiredPerDay!.caloriesNeeded = userKcal!.caloriesNeeded * 1.9;
            }

            if (daysForExercise == 0) { kcalRequiredPerDay!.caloriesNeeded = userKcal!.caloriesNeeded * 1.2; }

            ValidationValuesDB.ValidationValues(kcalRequiredPerDay!);

            if (!Save()) { throw new UnstoredValuesException(); }
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
    }
}
