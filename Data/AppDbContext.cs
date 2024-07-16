using Microsoft.EntityFrameworkCore;
using AppVidaSana.Models;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Models.Alimentación;
using AppVidaSana.Models.Seguimientos_Mensuales.Respuestas;
using AppVidaSana.Models.Seguimientos_Mensuales.Resultados;
using AppVidaSana.Models.Alimentación.Alimentos;
using AppVidaSana.Models.Habitos;

namespace AppVidaSana.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        public DbSet<Account> Accounts { get; set; }
        public DbSet<Profiles> Profiles { get; set; }

        public DbSet<Exercise> Exercises { get; set; }
        public DbSet<MFUsExcercise> MFUsExcercise { get; set; }

        public DbSet<Breakfast> Breakfasts { get; set; }
        public DbSet<Lunch> Lunchs { get; set; }
        public DbSet<Meal> Meals { get; set; }
        public DbSet<Snack> Snacks { get; set; }
        public DbSet<Dinner> Dinners { get; set; }
        public DbSet<FoodsLunch> foodsLunch { get; set; }
        public DbSet<FoodsBreakfast> foodsBreakfast { get; set; }
        public DbSet<FoodsMeal> foodsMeal { get; set; }
        public DbSet<FoodsSnack> foodsSnack { get; set; }
        public DbSet<FoodsDinner> foodsDinner { get; set; }
        public DbSet<MFUsNutrition> MFUsNutrition { get; set; }
        public DbSet<NutritionResults> resultsNutrition { get; set; }

        public DbSet<Medicamento> Medications { get; set; }
        public DbSet<SideEffect> sideEffects { get; set; }
        public DbSet<MFUsMedications> MFUsMedications { get; set; }

        public DbSet<DrinkHabit> habitsDrink { get; set; }
        public DbSet<DrugsHabit> habitsDrugs { get; set; }
        public DbSet<SleepHabit> habitsSleep { get; set; }
        public DbSet<MFUsHabits> MFUsHabits { get; set; }
        public DbSet<HabitsResults> resultsHabits { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //Cuenta_Perfil
            modelBuilder.Entity<Account>()
                .HasOne(account => account.profile)
                .WithOne(profile => profile.account)
                .HasForeignKey<Profiles>(profile => profile.accountID)
                .OnDelete(DeleteBehavior.Restrict);

            //Ejercicio y SegMenEjercicio
            modelBuilder.Entity<Account>()
                .HasMany(account => account.exercises)
                .WithOne(exercises => exercises.account)
                .HasForeignKey(exercises => exercises.accountID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.MFUsExercise)
                .WithOne(MFUsExercise => MFUsExercise.account)
                .HasForeignKey(MFUsExercise => MFUsExercise.accountID)
                .OnDelete(DeleteBehavior.Restrict);

            //Alimentacion(Desayuno, ..., Cena)
            modelBuilder.Entity<Account>()
                .HasMany(account => account.breakfasts)
                .WithOne(breakfasts => breakfasts.account)
                .HasForeignKey(breakfasts => breakfasts.accountID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.lunches)
                .WithOne(lunches => lunches.account)
                .HasForeignKey(lunches => lunches.accountID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.meals)
                .WithOne(meals => meals.account)
                .HasForeignKey(meals => meals.accountID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.snacks)
                .WithOne(snacks => snacks.account)
                .HasForeignKey(snacks => snacks.accountID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.dinners)
                .WithOne(dinners => dinners.account)
                .HasForeignKey(dinners => dinners.accountID)
                .OnDelete(DeleteBehavior.Restrict);

            //Alimentos (Desayuno, ..., Cena)
            modelBuilder.Entity<Breakfast>()
                .HasMany(breakfast => breakfast.foodsBreakfast)
                .WithOne(foods => foods.breakfast)
                .HasForeignKey(foods => foods.breakfastID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Lunch>()
                .HasMany(lunch => lunch.foodsLunch)
                .WithOne(foods => foods.lunch)
                .HasForeignKey(foods => foods.lunchID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Meal>()
                .HasMany(meal => meal.foodsMeal)
                .WithOne(foods => foods.meal)
                .HasForeignKey(foods => foods.mealID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Snack>()
                .HasMany(snack => snack.foodsSnack)
                .WithOne(foods => foods.snack)
                .HasForeignKey(foods => foods.snackID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Dinner>()
                .HasMany(dinner => dinner.foodsDinner)
                .WithOne(foods => foods.dinner)
                .HasForeignKey(foods => foods.dinnerID)
                .OnDelete(DeleteBehavior.Restrict);


            //SegMenAlimentos
            modelBuilder.Entity<Account>()
                .HasMany(account => account.MFUsNutrition)
                .WithOne(MFUsNutrition => MFUsNutrition.account)
                .HasForeignKey(MFUsNutrition => MFUsNutrition.accountID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MFUsNutrition>()
                .HasOne(MFUsNutrition => MFUsNutrition.results)
                .WithOne(results => results.MFUsNutrition)
                .HasForeignKey<NutritionResults>(results => results.monthlyFollowUpID)
                .OnDelete(DeleteBehavior.Restrict);


            //Medicamentos 
            modelBuilder.Entity<Account>()
                .HasMany(account => account.medications)
                .WithOne(medications => medications.account)
                .HasForeignKey(medications => medications.accountID)
                .OnDelete(DeleteBehavior.Restrict);

            //SegMenMedicamentos
            modelBuilder.Entity<Account>()
                .HasMany(account => account.MFUsMedications)
                .WithOne(MFUsMedications => MFUsMedications.account)
                .HasForeignKey(MFUsMedications => MFUsMedications.accountID)
                .OnDelete(DeleteBehavior.Restrict);

            //Efectos secundarios
            modelBuilder.Entity<Account>()
               .HasMany(account => account.sideEffects)
               .WithOne(sideEf => sideEf.account)
               .HasForeignKey(sideEf => sideEf.accountID)
               .OnDelete(DeleteBehavior.Restrict);

            //Habitos(Sueño, Bebidas, Drogas)
            modelBuilder.Entity<Account>()
                .HasMany(account => account.habitsSleep)
                .WithOne(habits => habits.account)
                .HasForeignKey(habits => habits.accountID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.habitsDrink)
                .WithOne(habits => habits.account)
                .HasForeignKey(habits => habits.accountID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Account>()
                .HasMany(account => account.habitsDrugs)
                .WithOne(habits => habits.account)
                .HasForeignKey(habits => habits.accountID)
                .OnDelete(DeleteBehavior.Restrict);



            //Seguimiento Mensual Habitos
            modelBuilder.Entity<Account>()
                .HasMany(account => account.MFUsHabits)
                .WithOne(MFUsHabits => MFUsHabits.account)
                .HasForeignKey(MFUsHabits => MFUsHabits.accountID)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MFUsHabits>()
                .HasOne(MFUsHabits => MFUsHabits.results)
                .WithOne(results => results.MFUsHabits)
                .HasForeignKey<HabitsResults>(results => results.monthlyFollowUpID)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
