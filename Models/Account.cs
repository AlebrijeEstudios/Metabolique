using AppVidaSana.Models.Alimentación;
using AppVidaSana.Models.Habitos;
using AppVidaSana.Models.Seguimientos_Mensuales;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models
{
    public class Account
    {
        [Key]
        public Guid accountID { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El campo username es obligatorio")]
        public string username { get; set; } = null!;

        [Required(ErrorMessage = "El campo email es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe tener la estructura de un correo.")]
        public string email { get; set; } = null!;

        [Required(ErrorMessage = "El campo contraseña es obligatoria")]
        public string password { get; set; } = null!;

        public string role { get; set; } = "User";

        public Profiles? profile { get; set; }

        public ICollection<Breakfast> breakfasts { get; set; } = new List<Breakfast>();

        public ICollection<Lunch> lunches { get; set; } = new List<Lunch>();

        public ICollection<Meal> meals { get; set; } = new List<Meal>();

        public ICollection<Snack> snacks { get; set; } = new List<Snack>();

        public ICollection<Dinner> dinners { get; set; } = new List<Dinner>();

        public ICollection<MFUsNutrition> MFUsNutrition { get; set; } = new List<MFUsNutrition>();

        public ICollection<Exercise> exercises { get; set; } = new List<Exercise>();

        public ICollection<MFUsExcercise> MFUsExercise { get; set; } = new List<MFUsExcercise>();

        public ICollection<Medicamento> medications { get; set; } = new List<Medicamento>();
        
        public ICollection<SideEffect> sideEffects { get; set; } = new List<SideEffect>();

        public ICollection<MFUsMedications> MFUsMedications { get; set; } = new List<MFUsMedications>();

        public ICollection<DrinkHabit> habitsDrink { get; set; } = new List<DrinkHabit>();

        public ICollection<DrugsHabit> habitsDrugs { get; set; } = new List<DrugsHabit>();

        public ICollection<SleepHabit> habitsSleep { get; set; } = new List<SleepHabit>();

        public ICollection<MFUsHabits> MFUsHabits { get; set; } = new List<MFUsHabits>();

    }
}
