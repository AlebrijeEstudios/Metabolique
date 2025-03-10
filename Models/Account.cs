using AppVidaSana.Models.Exercises;
using AppVidaSana.Models.Feeding;
using AppVidaSana.Models.Habits;
using AppVidaSana.Models.Medications;
using AppVidaSana.Models.Monthly_Follow_Ups;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models
{
    public class Account
    {
        [Key]
        public Guid accountID { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El campo username es obligatorio.")]
        public string username { get; set; } = null!;

        [Required(ErrorMessage = "El campo email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe tener la estructura de un correo.")]
        public string email { get; set; } = null!;

        [Required(ErrorMessage = "El campo contraseña es obligatoria.")]
        public string password { get; set; } = null!;

        public Profiles? profile { get; set; }

        public UserCalories? userCalories { get; set; }

        public HistorialRefreshToken? historialRefreshToken { get; set; }

        public ICollection<PacientDoctor> pacientDoctor { get; set; } = new List<PacientDoctor>();

        public ICollection<UserFeeds> userFeeds { get; set; } = new List<UserFeeds>();
        public ICollection<CaloriesConsumed> caloriesConsumed { get; set; } = new List<CaloriesConsumed>();
        public ICollection<CaloriesRequiredPerDay> caloriesRequiredPerDays { get; set; } = new List<CaloriesRequiredPerDay>();
        public ICollection<MFUsFood> MFUsFood { get; set; } = new List<MFUsFood>();

        public ICollection<Exercise> exercises { get; set; } = new List<Exercise>();
        public ICollection<ActiveMinutes> activeMinutes { get; set; } = new List<ActiveMinutes>();
        public ICollection<MFUsExercise> MFUsExercise { get; set; } = new List<MFUsExercise>();

        public ICollection<DrinkHabit> habitsDrink { get; set; } = new List<DrinkHabit>();
        public ICollection<DrugsHabit> habitsDrugs { get; set; } = new List<DrugsHabit>();
        public ICollection<SleepHabit> habitsSleep { get; set; } = new List<SleepHabit>();
        public ICollection<MFUsHabits> MFUsHabits { get; set; } = new List<MFUsHabits>();

        public ICollection<PeriodsMedications> periodsMedications { get; set; } = new List<PeriodsMedications>();
        public ICollection<SideEffects> sideEffects { get; set; } = new List<SideEffects>();
        public ICollection<MFUsMedication> MFUsMedications { get; set; } = new List<MFUsMedication>();

    }
}
