using Microsoft.EntityFrameworkCore;
using AppVidaSana.Models;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Models.Seguimientos_Mensuales.Resultados;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Models.Medications;
using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Models.Exercises;
using AppVidaSana.Models.Monthly_Follow_Ups.Results;

namespace AppVidaSana.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<MFUsMonths> Months { get; set; }
        public DbSet<Account> Accounts { get; set; }
        public DbSet<Roles> Roles { get; set; }
        public DbSet<Profiles> Profiles { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<MFUsExercise> MFUsExercise { get; set; }
        public DbSet<ExerciseResults> ResultsExercise { get; set; }
        public DbSet<minutesConsumed> MinutesConsumed {  get; set; }
        public DbSet<DrinkHabit> HabitsDrink { get; set; }
        public DbSet<DrugsHabit> HabitsDrugs { get; set; }
        public DbSet<SleepHabit> HabitsSleep { get; set; }
        public DbSet<MFUsHabits> MFUsHabits { get; set; }
        public DbSet<HabitsResults> ResultsHabits { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<MFUsMedication> MFUsMedication { get; set; }
        public DbSet<Times> Times { get; set; }
        public DbSet<SideEffects> SideEffects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Account-Profile
            modelBuilder.Entity<Account>()
                .HasOne(account => account.profile)
                .WithOne(profile => profile.account)
                .HasForeignKey<Profiles>(profile => profile.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Roles>().HasData(
                new Roles { roleID = Guid.NewGuid(), role = "User" },
                new Roles { roleID = Guid.NewGuid(), role = "Admin" }
            );

            modelBuilder.Entity<Roles>()
                .HasOne(rol => rol.account)
                .WithOne(account => account.roles)
                .HasForeignKey<Account>(account => account.roleID)
                .OnDelete(DeleteBehavior.Restrict);

            //Exercise and MFUsExercise
            modelBuilder.Entity<Account>()
                .HasMany(account => account.exercises)
                .WithOne(exercises => exercises.account)
                .HasForeignKey(exercises => exercises.accountID)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<Account>()
                .HasMany(account => account.graphicsValuesExercise)
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
                .HasMany(account => account.medications)
                .WithOne(medications => medications.account)
                .HasForeignKey(medications => medications.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.times)
                .WithOne(times => times.account)
                .HasForeignKey(times => times.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Medication>()
                .HasMany(medication => medication.times)
                .WithOne(times => times.medication)
                .HasForeignKey(times => times.medicationID)
                .OnDelete(DeleteBehavior.NoAction);

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

            //SideEffects
            modelBuilder.Entity<Account>()
                .HasMany(account => account.sideEffects)
                .WithOne(sideEffects => sideEffects.account)
                .HasForeignKey(sideEffects => sideEffects.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Medication>()
                .HasMany(medication => medication.sideEffects)
                .WithOne(sideEffects => sideEffects.medication)
                .HasForeignKey(sideEffects => sideEffects.medicationID)
                .OnDelete(DeleteBehavior.NoAction);
        }
    }
}
