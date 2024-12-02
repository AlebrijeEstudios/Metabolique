using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Feeding
{
    public class CaloriesConsumed
    {
        [Key]
        public Guid caloriesConsumedID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatoria")]
        public DateOnly dateCaloriesConsumed { get; set; }

        [Required(ErrorMessage = "El campo tiempo total es obligatorio")]
        public float totalCaloriesConsumed { get; set; }

        public Account? account { get; set; }
    }
}
