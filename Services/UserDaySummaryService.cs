using AppVidaSana.Data;
using AppVidaSana.Models.Dtos;
using AppVidaSana.Models.Dtos.UserDaysSummary_Dtos;
using AppVidaSana.Models.Medications;
using AppVidaSana.Services.IServices;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Services
{
    public class UserDaySummaryService : IUserDaySummary
    {
        private readonly AppDbContext _bd;
        private readonly IMapper _mapper;

        public UserDaySummaryService(AppDbContext bd, IMapper mapper)
        {
            _bd = bd;
            _mapper = mapper;
        }

        public async Task<UserDaySummaryDto> GetUserDaySummaryAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {
            var userName = await GetUserNameAsync(accountID, cancellationToken);

            var caloriesSummary = await GetCaloriesSummaryAsync(accountID, date, cancellationToken);

            var timeExerciseSummary = await GetTimeExerciseSummaryAsync(accountID, date, cancellationToken);

            var timeSleepSummary = await GetTimeSleepSummaryAsync(accountID, date, cancellationToken);

            var medicationSummary = GetMedicationSummaryAsync(accountID, date);

            UserDaySummaryDto userDaySummary = new UserDaySummaryDto
            {
                userName = userName,
                calories = caloriesSummary,
                timeExercise = timeExerciseSummary,
                timeSleep = timeSleepSummary,
                medications = medicationSummary
            };

            return userDaySummary;
        }

        private async Task<string> GetUserNameAsync(Guid accountID, CancellationToken cancellationToken)
        {
            var userName = await _bd.Accounts.FindAsync(new object[] { accountID }, cancellationToken);

            if (userName is null) { return "null"; } 

            return userName.username; 
        }

        private async Task<CaloriesSummaryDto> GetCaloriesSummaryAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {
            CaloriesSummaryDto calories = new CaloriesSummaryDto();

            var caloriesToConsume = await _bd.CaloriesRequiredPerDays.FirstOrDefaultAsync(e => e.accountID == accountID
                                                                                          && e.dateInitial <= date
                                                                                          && date <= e.dateFinal, cancellationToken);

            var caloriesConsumed = await _bd.CaloriesConsumed.FirstOrDefaultAsync(e => e.accountID == accountID
                                                                                  && e.dateCaloriesConsumed == date, 
                                                                                  cancellationToken);

            if (caloriesToConsume is not null)
            {
                calories.limit = caloriesToConsume.caloriesNeeded;
            }

            if (caloriesConsumed is not null)
            {
                calories.value = caloriesConsumed.totalCaloriesConsumed;
            }

            return calories;
        }

        private async Task<int> GetTimeExerciseSummaryAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {
            var activeMinutes = await _bd.ActiveMinutes.FirstOrDefaultAsync(e => e.accountID == accountID && e.dateExercise == date,
                                                                            cancellationToken);

            if(activeMinutes is not null)
            {
                return activeMinutes.totalTimeSpent;
            }

            return 0;
        }

        private async Task<int?> GetTimeSleepSummaryAsync(Guid accountID, DateOnly date, CancellationToken cancellationToken)
        {
            var hoursSleep = await _bd.HabitsSleep.FirstOrDefaultAsync(e => e.accountID == accountID && e.sleepDateHabit == date,
                                                                       cancellationToken);

            if(hoursSleep is not null)
            {
                if (hoursSleep.sleepHours == null) { return 0; }

                return hoursSleep.sleepHours;
            }

            return 0;
        }

        private MedicationSummaryDto GetMedicationSummaryAsync(Guid accountID, DateOnly date)
        {
            MedicationSummaryDto medications = new MedicationSummaryDto();

            var timesPeriod = from pMed in _bd.Set<PeriodsMedications>()
                              join t in _bd.Set<Times>()
                              on pMed.periodID equals t.periodID
                              where pMed.accountID == accountID && t.dateMedication == date
                              orderby t.time
                              select t;

            var timeList = timesPeriod.ToList();

            if(timeList.Count > 0)
            {
                medications.limit = timeList.Count;
                medications.value = timeList.Count(e => e.medicationStatus);

                return medications;
            }

            return medications;
        }
    }
}
