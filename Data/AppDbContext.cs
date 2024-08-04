using Microsoft.EntityFrameworkCore;
using AppVidaSana.Models;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Models.Seguimientos_Mensuales.Resultados;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Models.Graphics;
using AppVidaSana.Models.Medications;
using AppVidaSana.Models.Monthly_Follow_Ups;

namespace AppVidaSana.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Profiles> Profiles { get; set; }
        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<MFUsExercise> MFUsExercise { get; set; }
        public DbSet<GraphicsValuesExercise> graphicsValuesExercise {  get; set; }
        public DbSet<DrinkHabit> habitsDrink { get; set; }
        public DbSet<DrugsHabit> habitsDrugs { get; set; }
        public DbSet<SleepHabit> habitsSleep { get; set; }
        public DbSet<MFUsHabits> MFUsHabits { get; set; }
        public DbSet<HabitsResults> resultsHabits { get; set; }
        public DbSet<Medication> Medications { get; set; }
        public DbSet<MFUsMedication> MFUsMedication { get; set; }
        public DbSet<Times> Times { get; set; }
        public DbSet<SideEffects> sideEffects { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Cuenta_Perfil
            modelBuilder.Entity<Account>()
                .HasOne(account => account.profile)
                .WithOne(profile => profile.account)
                .HasForeignKey<Profiles>(profile => profile.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            //Ejercicio y SegMenEjercicio
            modelBuilder.Entity<Account>()
                .HasMany(account => account.exercises)
                .WithOne(exercises => exercises.account)
                .HasForeignKey(exercises => exercises.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.MFUsExercise)
                .WithOne(MFUsExercise => MFUsExercise.account)
                .HasForeignKey(MFUsExercise => MFUsExercise.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.graphicsValuesExercise)
                .WithOne(graphic => graphic.account)
                .HasForeignKey(graphic => graphic.accountID)
                .OnDelete(DeleteBehavior.Cascade);

            //Habitos(Sueño, Bebidas, Drogas)
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

            //Seguimiento Mensual Habitos
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

            modelBuilder.Entity<MFUsHabits>()
               .Property(e => e.answerQuestion1)
               .HasColumnType("TIME(0)");

            modelBuilder.Entity<MFUsHabits>()
              .Property(e => e.answerQuestion3)
              .HasColumnType("TIME(0)");

            //Medicamentos
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
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.MFUsMedications)
                .WithOne(MFUsMedications => MFUsMedications.account)
                .HasForeignKey(MFUsMedications => MFUsMedications.accountID)
                .OnDelete(DeleteBehavior.Cascade);

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
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
