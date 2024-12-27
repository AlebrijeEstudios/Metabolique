using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Feeding
{
    public class CaloriesRequiredPerDay
    {
        [Key]
        public Guid caloriesPerDayID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo fecha inicial es obligatorio")]
        public DateOnly dateInitial { get; set; }

        [Required(ErrorMessage = "El campo fecha final es obligatorio")]
        public DateOnly dateFinal { get; set; }

        [Required(ErrorMessage = "El campo calorias necesarias es obligatorio")]
        public double caloriesNeeded { get; set; }

        public Account? account { get; set; }
    }
}
