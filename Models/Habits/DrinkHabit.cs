using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Habits
{
    public class DrinkHabit
    {
        [Key]
        public Guid drinkHabitID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        public DateOnly drinkDateHabit { get; set; }

        public int? amountConsumed { get; set; }

        public Account? account { get; set; }
    }
}
