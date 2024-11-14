using AppVidaSana.Models;
using AppVidaSana.Models.Exercises;
using AppVidaSana.Models.Food;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Models.Medications;
using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Models.Monthly_Follow_Ups.Results;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Models.Seguimientos_Mensuales.Resultados;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<HistorialRefreshToken> HistorialRefreshTokens { get; set; }
        public DbSet<MFUsMonths> Months { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Profiles> Profiles { get; set; }
        public DbSet<UserFeeds> UserFeeds { get; set; }
        public DbSet<FoodConsumed> FoodsConsumed { get; set; }
        public DbSet<DailyMeals> DailyMeals { get; set; }
        public DbSet<MFUsFood> MFUsFood { get; set; }
        public DbSet<FoodResults> ResultsFood { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<MFUsExercise> MFUsExercise { get; set; }
        public DbSet<ExerciseResults> ResultsExercise { get; set; }
        public DbSet<ActiveMinutes> ActiveMinutes { get; set; }
        public DbSet<DrinkHabit> HabitsDrink { get; set; }
        public DbSet<DrugsHabit> HabitsDrugs { get; set; } 
        public DbSet<SleepHabit> HabitsSleep { get; set; } 
        public DbSet<MFUsHabits> MFUsHabits { get; set; }
        public DbSet<HabitsResults> ResultsHabits { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<MFUsMedication> MFUsMedication { get; set; }
        public DbSet<PeriodsMedications> PeriodsMedications { get; set; }
        public DbSet<Times> Times { get; set; }
        public DbSet<SideEffects> SideEffects { get; set; }
        public DbSet<StatusAdherence> StatusAdherence { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>()
                .HasOne(account => account.historialRefreshToken)
                .WithOne(historial => historial.account)
                .HasForeignKey<HistorialRefreshToken>(historial => historial.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            //Account-Profile
            modelBuilder.Entity<Account>()
                .HasOne(account => account.profile)
                .WithOne(profile => profile.account)
                .HasForeignKey<Profiles>(profile => profile.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Roles>().HasData(
                new Roles { roleID = Guid.Parse("2bc6e28a-7fbb-4649-aa30-f1f3e3202f81"), role = "User" },
                new Roles { roleID = Guid.Parse("bd73f55e-7cac-4683-84c1-2fa7e2dc6edb"), role = "Admin" }
            );

            modelBuilder.Entity<Roles>()
                .HasMany(rol => rol.account)
                .WithOne(account => account.roles)
                .HasForeignKey(account => account.roleID)
                .OnDelete(DeleteBehavior.Restrict);

            //Food and MFUsFood
            modelBuilder.Entity<Account>()
                .HasMany(account => account.userFeeds)
                .WithOne(userFeeds => userFeeds.account)
                .HasForeignKey(userFeeds => userFeeds.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<DailyMeals>()
                .HasMany(dailyMeal => dailyMeal.userFeeds)
                .WithOne(userFeeds => userFeeds.dailyMeals)
                .HasForeignKey(userFeeds => userFeeds.dailyMealID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFeeds>()
                .HasMany(userFeed => userFeed.foodsConsumed)
                .WithOne(foodConsumed => foodConsumed.userFeeds)
                .HasForeignKey(foodConsumed => foodConsumed.userFeedID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.caloriesConsumed)
                .WithOne(graphic => graphic.account)
                .HasForeignKey(graphic => graphic.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.MFUsFood)
                .WithOne(MFUsFood => MFUsFood.account)
                .HasForeignKey(MFUsFood => MFUsFood.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MFUsMonths>()
               .HasMany(month => month.foods)
               .WithOne(foods => foods.months)
               .HasForeignKey(foods => foods.monthID)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MFUsFood>()
                .HasOne(MFUsFood => MFUsFood.results)
                .WithOne(results => results.MFUsFood)
                .HasForeignKey<FoodResults>(results => results.monthlyFollowUpID)
                .OnDelete(DeleteBehavior.Cascade);

            //Exercise and MFUsExercise
            modelBuilder.Entity<Account>()
                .HasMany(account => account.exercises)
                .WithOne(exercises => exercises.account)
                .HasForeignKey(exercises => exercises.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.activeMinutes)
                .WithOne(graphic => graphic.account)
                .HasForeignKey(graphic => graphic.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.MFUsExercise)
                .WithOne(MFUsExercise => MFUsExercise.account)
                .HasForeignKey(MFUsExercise => MFUsExercise.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MFUsMonths>()
               .HasMany(month => month.exercises)
               .WithOne(exercises => exercises.months)
               .HasForeignKey(exercises => exercises.monthID)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MFUsExercise>()
                .HasOne(MFUsExercise => MFUsExercise.results)
                .WithOne(results => results.MFUsExercise)
                .HasForeignKey<ExerciseResults>(results => results.monthlyFollowUpID)
                .OnDelete(DeleteBehavior.Cascade);

            //Habits (Drugs, Drink, Sleep)
            modelBuilder.Entity<Account>()
                .HasMany(account => account.habitsSleep)
                .WithOne(habits => habits.account)
                .HasForeignKey(habits => habits.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.habitsDrink)
                .WithOne(habits => habits.account)
                .HasForeignKey(habits => habits.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.habitsDrugs)
                .WithOne(habits => habits.account)
                .HasForeignKey(habits => habits.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            //MFUsHabits
            modelBuilder.Entity<Account>()
                .HasMany(account => account.MFUsHabits)
                .WithOne(MFUsHabits => MFUsHabits.account)
                .HasForeignKey(MFUsHabits => MFUsHabits.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MFUsHabits>()
                .HasOne(MFUsHabits => MFUsHabits.results)
                .WithOne(results => results.MFUsHabits)
                .HasForeignKey<HabitsResults>(results => results.monthlyFollowUpID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MFUsMonths>()
                .HasMany(month => month.habits)
                .WithOne(habits => habits.months)
                .HasForeignKey(habits => habits.monthID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MFUsHabits>()
               .Property(e => e.answerQuestion1)
               .HasColumnType("TIME(0)");

            modelBuilder.Entity<MFUsHabits>()
              .Property(e => e.answerQuestion3)
              .HasColumnType("TIME(0)");

            //Medications
            modelBuilder.Entity<Account>()
                .HasMany(account => account.periodsMedications)
                .WithOne(periodsMedications => periodsMedications.account)
                .HasForeignKey(periodsMedications => periodsMedications.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Medication>()
                .HasMany(medication => medication.periods)
                .WithOne(periods => periods.medication)
                .HasForeignKey(periods => periods.medicationID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<PeriodsMedications>()
                .HasMany(periods => periods.times)
                .WithOne(times => times.periods)
                .HasForeignKey(times => times.periodID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Times>()
              .Property(e => e.time)
              .HasColumnType("TIME(0)");

            //SideEffects
            modelBuilder.Entity<Account>()
                .HasMany(account => account.sideEffects)
                .WithOne(sideEffects => sideEffects.account)
                .HasForeignKey(sideEffects => sideEffects.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<SideEffects>()
              .Property(e => e.initialTime)
              .HasColumnType("TIME(0)");

            modelBuilder.Entity<SideEffects>()
              .Property(e => e.finalTime)
              .HasColumnType("TIME(0)");

            //MFUsMedications
            modelBuilder.Entity<Account>()
                .HasMany(account => account.MFUsMedications)
                .WithOne(MFUsMedications => MFUsMedications.account)
                .HasForeignKey(MFUsMedications => MFUsMedications.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<MFUsMonths>()
               .HasMany(month => month.medications)
               .WithOne(medications => medications.months)
               .HasForeignKey(medications => medications.monthID)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StatusAdherence>().HasData(
                new StatusAdherence { statusID = Guid.Parse("3f9cb946-e43d-4865-b451-aa2f2da0f6b7"), statusAdherence = "Positivo" },
                new StatusAdherence { statusID = Guid.Parse("44204900-efa7-4b49-8c49-ba5c7450c983"), statusAdherence = "Negativo" }
            );

            modelBuilder.Entity<StatusAdherence>()
                .HasMany(status => status.mfuMed)
                .WithOne(mfuMed => mfuMed.status)
                .HasForeignKey(mfuMed => mfuMed.statusID)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
