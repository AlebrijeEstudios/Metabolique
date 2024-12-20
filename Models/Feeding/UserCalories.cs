using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Feeding
{
    public class UserCalories
    {
        [Key]
        public Guid userCaloriesID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo calorias necesarias es obligatorio")]
        public double caloriesNeeded { get; set; }

        public Account? account { get; set; }
    }
}
