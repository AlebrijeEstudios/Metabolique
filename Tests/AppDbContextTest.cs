using AppVidaSana.Models.Alimentación.Alimentos;
using AppVidaSana.Models.Alimentación;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Models.Seguimientos_Mensuales.Respuestas;
using AppVidaSana.Models.Seguimientos_Mensuales.Resultados;
using AppVidaSana.Models.Seguimientos_Mensuales;
using AppVidaSana.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AppVidaSana.Tests
{
    public class AppDbContextTest : IDisposable
    {
        private readonly DbContextOptions<TestDbContext> _options;

        public AppDbContextTest()
        {
            _options = new DbContextOptionsBuilder<TestDbContext>()
                .UseInMemoryDatabase(databaseName: "TestDatabase")
                .Options;

            using (var context = new TestDbContext(_options))
            {
                context.Database.EnsureCreated();
            }
        }

        public void Dispose()
        {
            using (var context = new TestDbContext(_options))
            {
                context.Database.EnsureDeleted();
            }
        }

        public class TestDbContext : DbContext
        {
            public TestDbContext(DbContextOptions<TestDbContext> options) : base(options) { }

            public DbSet<Account> Accounts { get; set; }
            public DbSet<Profiles> Profiles { get; set; }
            public DbSet<Exercise> Exercises { get; set; }
            public DbSet<MFUsExercise> MFUsExercise { get; set; }
            public DbSet<Breakfast> Breakfasts { get; set; }
            public DbSet<Lunch> Lunchs { get; set; }
            public DbSet<Meal> Meals { get; set; }
            public DbSet<Snack> Snacks { get; set; }
            public DbSet<Dinner> Dinners { get; set; }
            public DbSet<FoodsLunch> FoodsLunch { get; set; }
            public DbSet<FoodsBreakfast> FoodsBreakfast { get; set; }
            public DbSet<FoodsMeal> FoodsMeal { get; set; }
            public DbSet<FoodsSnack> FoodsSnack { get; set; }
            public DbSet<FoodsDinner> FoodsDinner { get; set; }
            public DbSet<MFUsNutrition> MFUsNutrition { get; set; }
            public DbSet<NutritionResults> NutritionResults { get; set; }
            public DbSet<Medication> Medications { get; set; }
            public DbSet<SideEffect> SideEffects { get; set; }
            public DbSet<MFUsMedications> MFUsMedications { get; set; }
            public DbSet<DrinkHabit> HabitsDrink { get; set; }
            public DbSet<DrugsHabit> HabitsDrugs { get; set; }
            public DbSet<SleepHabit> HabitsSleep { get; set; }
            public DbSet<MFUsHabits> MFUsHabits { get; set; }
            public DbSet<HabitsResults> HabitsResults { get; set; }

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

                //Alimentacion(Desayuno, ..., Cena)
                modelBuilder.Entity<Account>()
                    .HasMany(account => account.breakfasts)
                    .WithOne(breakfasts => breakfasts.account)
                    .HasForeignKey(breakfasts => breakfasts.accountID)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<Account>()
                    .HasMany(account => account.lunches)
                    .WithOne(lunches => lunches.account)
                    .HasForeignKey(lunches => lunches.accountID)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<Account>()
                    .HasMany(account => account.meals)
                    .WithOne(meals => meals.account)
                    .HasForeignKey(meals => meals.accountID)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<Account>()
                    .HasMany(account => account.snacks)
                    .WithOne(snacks => snacks.account)
                    .HasForeignKey(snacks => snacks.accountID)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<Account>()
                    .HasMany(account => account.dinners)
                    .WithOne(dinners => dinners.account)
                    .HasForeignKey(dinners => dinners.accountID)
                    .OnDelete(DeleteBehavior.Cascade);

                //Alimentos (Desayuno, ..., Cena)
                modelBuilder.Entity<Breakfast>()
                    .HasMany(breakfast => breakfast.foodsBreakfast)
                    .WithOne(foods => foods.breakfast)
                    .HasForeignKey(foods => foods.breakfastID)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<Lunch>()
                    .HasMany(lunch => lunch.foodsLunch)
                    .WithOne(foods => foods.lunch)
                    .HasForeignKey(foods => foods.lunchID)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<Meal>()
                    .HasMany(meal => meal.foodsMeal)
                    .WithOne(foods => foods.meal)
                    .HasForeignKey(foods => foods.mealID)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<Snack>()
                    .HasMany(snack => snack.foodsSnack)
                    .WithOne(foods => foods.snack)
                    .HasForeignKey(foods => foods.snackID)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<Dinner>()
                    .HasMany(dinner => dinner.foodsDinner)
                    .WithOne(foods => foods.dinner)
                    .HasForeignKey(foods => foods.dinnerID)
                    .OnDelete(DeleteBehavior.Cascade);


                //SegMenAlimentos   
                modelBuilder.Entity<Account>()
                    .HasMany(account => account.MFUsNutrition)
                    .WithOne(MFUsNutrition => MFUsNutrition.account)
                    .HasForeignKey(MFUsNutrition => MFUsNutrition.accountID)
                    .OnDelete(DeleteBehavior.Cascade);

                modelBuilder.Entity<MFUsNutrition>()
                    .HasOne(MFUsNutrition => MFUsNutrition.results)
                    .WithOne(results => results.MFUsNutrition)
                    .HasForeignKey<NutritionResults>(results => results.monthlyFollowUpID)
                    .OnDelete(DeleteBehavior.Cascade);


                //Medicamentos 
                modelBuilder.Entity<Account>()
                    .HasMany(account => account.medications)
                    .WithOne(medications => medications.account)
                    .HasForeignKey(medications => medications.accountID)
                    .OnDelete(DeleteBehavior.Cascade);

                //SegMenMedicamentos
                modelBuilder.Entity<Account>()
                    .HasMany(account => account.MFUsMedications)
                    .WithOne(MFUsMedications => MFUsMedications.account)
                    .HasForeignKey(MFUsMedications => MFUsMedications.accountID)
                    .OnDelete(DeleteBehavior.Cascade);

                //Efectos secundarios
                modelBuilder.Entity<Account>()
                   .HasMany(account => account.sideEffects)
                   .WithOne(sideEf => sideEf.account)
                   .HasForeignKey(sideEf => sideEf.accountID)
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

            }
        }

        [Fact]
        public void Test_Account_Profile_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var accountEntity = model.FindEntityType(typeof(Account));
                var profileEntity = model.FindEntityType(typeof(Profiles));

                var foreignKey = profileEntity.FindForeignKeys(profileEntity.FindProperty(nameof(Profiles.accountID)))
                    .Single();

                Assert.Equal(nameof(Profiles.accountID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(Profiles.account), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Account.profile), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Account_Exercise_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var accountEntity = model.FindEntityType(typeof(Account));
                var exerciseEntity = model.FindEntityType(typeof(Exercise));

                var foreignKey = exerciseEntity.FindForeignKeys(exerciseEntity.FindProperty(nameof(Exercise.accountID)))
                    .Single();

                Assert.Equal(nameof(Exercise.accountID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(Exercise.account), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Account.exercises), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Account_MFUsExercise_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var accountEntity = model.FindEntityType(typeof(Account));
                var MFUsexerciseEntity = model.FindEntityType(typeof(MFUsExercise));

                var foreignKey = MFUsexerciseEntity.FindForeignKeys(MFUsexerciseEntity.FindProperty(nameof(MFUsExercise.accountID)))
                    .Single();

                Assert.Equal(nameof(MFUsExercise.accountID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(MFUsExercise.account), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Account.MFUsExercise), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Account_Breakfasts_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var accountEntity = model.FindEntityType(typeof(Account));
                var breakfastEntity = model.FindEntityType(typeof(Breakfast));

                var foreignKey = breakfastEntity.FindForeignKeys(breakfastEntity.FindProperty(nameof(Breakfast.accountID)))
                    .Single();

                Assert.Equal(nameof(Breakfast.accountID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(Breakfast.account), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Account.breakfasts), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Account_Lunches_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var accountEntity = model.FindEntityType(typeof(Account));
                var lunchEntity = model.FindEntityType(typeof(Lunch));

                var foreignKey = lunchEntity.FindForeignKeys(lunchEntity.FindProperty(nameof(Lunch.accountID)))
                    .Single();

                Assert.Equal(nameof(Lunch.accountID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(Lunch.account), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Account.lunches), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Account_Meals_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var accountEntity = model.FindEntityType(typeof(Account));
                var mealEntity = model.FindEntityType(typeof(Meal));

                var foreignKey = mealEntity.FindForeignKeys(mealEntity.FindProperty(nameof(Meal.accountID)))
                    .Single();

                Assert.Equal(nameof(Meal.accountID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(Meal.account), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Account.meals), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Account_Snacks_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var accountEntity = model.FindEntityType(typeof(Account));
                var snackEntity = model.FindEntityType(typeof(Snack));

                var foreignKey = snackEntity.FindForeignKeys(snackEntity.FindProperty(nameof(Snack.accountID)))
                    .Single();

                Assert.Equal(nameof(Snack.accountID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(Snack.account), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Account.snacks), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Account_Dinners_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var accountEntity = model.FindEntityType(typeof(Account));
                var dinnerEntity = model.FindEntityType(typeof(Dinner));

                var foreignKey = dinnerEntity.FindForeignKeys(dinnerEntity.FindProperty(nameof(Dinner.accountID)))
                    .Single();

                Assert.Equal(nameof(Dinner.accountID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(Dinner.account), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Account.dinners), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Breakfast_Foods_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var foodEntity = model.FindEntityType(typeof(FoodsBreakfast));
                var breakfastEntity = model.FindEntityType(typeof(Breakfast));

                var foreignKey = foodEntity.FindForeignKeys(foodEntity.FindProperty(nameof(FoodsBreakfast.breakfastID)))
                    .Single();

                Assert.Equal(nameof(FoodsBreakfast.breakfastID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(FoodsBreakfast.breakfast), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Breakfast.foodsBreakfast), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Lunch_Foods_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var foodEntity = model.FindEntityType(typeof(FoodsLunch));
                var lunchEntity = model.FindEntityType(typeof(Lunch));

                var foreignKey = foodEntity.FindForeignKeys(foodEntity.FindProperty(nameof(FoodsLunch.lunchID)))
                    .Single();

                Assert.Equal(nameof(FoodsLunch.lunchID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(FoodsLunch.lunch), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Lunch.foodsLunch), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Meal_Foods_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var foodEntity = model.FindEntityType(typeof(FoodsMeal));
                var mealEntity = model.FindEntityType(typeof(Meal));

                var foreignKey = foodEntity.FindForeignKeys(foodEntity.FindProperty(nameof(FoodsMeal.mealID)))
                    .Single();

                Assert.Equal(nameof(FoodsMeal.mealID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(FoodsMeal.meal), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Meal.foodsMeal), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Snack_Foods_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var foodEntity = model.FindEntityType(typeof(FoodsSnack));
                var snackEntity = model.FindEntityType(typeof(Snack));

                var foreignKey = foodEntity.FindForeignKeys(foodEntity.FindProperty(nameof(FoodsSnack.snackID)))
                    .Single();

                Assert.Equal(nameof(FoodsSnack.snackID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(FoodsSnack.snack), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Snack.foodsSnack), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Dinner_Foods_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var foodEntity = model.FindEntityType(typeof(FoodsDinner));
                var dinnerEntity = model.FindEntityType(typeof(Dinner));

                var foreignKey = foodEntity.FindForeignKeys(foodEntity.FindProperty(nameof(FoodsDinner.dinnerID)))
                    .Single();

                Assert.Equal(nameof(FoodsDinner.dinnerID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(FoodsDinner.dinner), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Dinner.foodsDinner), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Account_MFUsNutrition_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var accountEntity = model.FindEntityType(typeof(Account));
                var MFUsnutritionEntity = model.FindEntityType(typeof(MFUsNutrition));

                var foreignKey = MFUsnutritionEntity.FindForeignKeys(MFUsnutritionEntity.FindProperty(nameof(MFUsNutrition.accountID)))
                    .Single();

                Assert.Equal(nameof(MFUsNutrition.accountID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(MFUsNutrition.account), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Account.MFUsNutrition), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Results_MFUsNutrition_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var resultEntity = model.FindEntityType(typeof(NutritionResults));
                var MFUsnutritionEntity = model.FindEntityType(typeof(MFUsNutrition));

                var foreignKey = resultEntity.FindForeignKeys(resultEntity.FindProperty(nameof(NutritionResults.monthlyFollowUpID)))
                    .Single();

                Assert.Equal(nameof(MFUsNutrition.monthlyFollowUpID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(NutritionResults.MFUsNutrition), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(MFUsNutrition.results), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Account_Medications_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var accountEntity = model.FindEntityType(typeof(Account));
                var mediEntity = model.FindEntityType(typeof(Medication));

                var foreignKey = mediEntity.FindForeignKeys(mediEntity.FindProperty(nameof(Medication.accountID)))
                    .Single();

                Assert.Equal(nameof(Medication.accountID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(Medication.account), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Account.medications), foreignKey.PrincipalToDependent.Name);
            }
        }


        [Fact]
        public void Test_Account_MFUsMedications_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var accountEntity = model.FindEntityType(typeof(Account));
                var MFUsmediEntity = model.FindEntityType(typeof(MFUsMedications));

                var foreignKey = MFUsmediEntity.FindForeignKeys(MFUsmediEntity.FindProperty(nameof(MFUsMedications.accountID)))
                    .Single();

                Assert.Equal(nameof(MFUsMedications.accountID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(MFUsMedications.account), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Account.MFUsMedications), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Account_SideEffect_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var accountEntity = model.FindEntityType(typeof(Account));
                var sideEfEntity = model.FindEntityType(typeof(SideEffect));

                var foreignKey = sideEfEntity.FindForeignKeys(sideEfEntity.FindProperty(nameof(SideEffect.accountID)))
                    .Single();

                Assert.Equal(nameof(SideEffect.accountID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(SideEffect.account), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Account.sideEffects), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Account_HSleep_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var accountEntity = model.FindEntityType(typeof(Account));
                var hSleepEntity = model.FindEntityType(typeof(SleepHabit));

                var foreignKey = hSleepEntity.FindForeignKeys(hSleepEntity.FindProperty(nameof(SleepHabit.accountID)))
                    .Single();

                Assert.Equal(nameof(SleepHabit.accountID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(SleepHabit.account), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Account.habitsSleep), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Account_HDrink_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var accountEntity = model.FindEntityType(typeof(Account));
                var hDrinkEntity = model.FindEntityType(typeof(DrinkHabit));

                var foreignKey = hDrinkEntity.FindForeignKeys(hDrinkEntity.FindProperty(nameof(DrinkHabit.accountID)))
                    .Single();

                Assert.Equal(nameof(DrinkHabit.accountID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(DrinkHabit.account), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Account.habitsDrink), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Account_HDrugs_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var accountEntity = model.FindEntityType(typeof(Account));
                var hDrugsEntity = model.FindEntityType(typeof(DrugsHabit));

                var foreignKey = hDrugsEntity.FindForeignKeys(hDrugsEntity.FindProperty(nameof(DrugsHabit.accountID)))
                    .Single();

                Assert.Equal(nameof(DrugsHabit.accountID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(DrugsHabit.account), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Account.habitsDrugs), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Account_MFUsHabits_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var accountEntity = model.FindEntityType(typeof(Account));
                var MFUshabitsEntity = model.FindEntityType(typeof(MFUsHabits));

                var foreignKey = MFUshabitsEntity.FindForeignKeys(MFUshabitsEntity.FindProperty(nameof(MFUsHabits.accountID)))
                    .Single();

                Assert.Equal(nameof(MFUsHabits.accountID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(MFUsHabits.account), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(Account.MFUsHabits), foreignKey.PrincipalToDependent.Name);
            }
        }

        [Fact]
        public void Test_Results_MFUsHabits_Relation()
        {

            using (var context = new TestDbContext(_options))
            {
                var model = context.Model;
                var resultEntity = model.FindEntityType(typeof(HabitsResults));
                var MFUshabitsEntity = model.FindEntityType(typeof(MFUsHabits));

                var foreignKey = resultEntity.FindForeignKeys(resultEntity.FindProperty(nameof(HabitsResults.monthlyFollowUpID)))
                    .Single();

                Assert.Equal(nameof(MFUsHabits.monthlyFollowUpID), foreignKey.Properties.Single().Name);
                Assert.Equal(DeleteBehavior.Cascade, foreignKey.DeleteBehavior);
                Assert.Equal(nameof(HabitsResults.MFUsHabits), foreignKey.DependentToPrincipal.Name);
                Assert.Equal(nameof(MFUsHabits.results), foreignKey.PrincipalToDependent.Name);
            }
        }


    }
}
