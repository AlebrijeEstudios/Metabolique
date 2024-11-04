using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Habitos
{
    public class DrinkHabit
    {
        [Key]
        public Guid drinkHabitID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        public DateOnly drinkDateHabit { get; set; }

        [Required(ErrorMessage = "El campo tipo de bebida es obligatorio")]
        public string typeDrink { get; set; } = null!;

        [Required(ErrorMessage = "El campo cantidad consumida es obligatorio")]
        public string amountConsumed { get; set; } = null!;

        public Account? account { get; set; }
    }
}
