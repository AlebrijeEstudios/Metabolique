using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Habitos
{
    public class DrugsHabit
    {
        [Key]
        public Guid drugsHabitID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        public DateOnly drugsDateHabit { get; set; }

        [Required(ErrorMessage = "El campo cigarros consumidos es obligatorio")]
        public int cigarettesSmoked { get; set; }

        [Required(ErrorMessage = "El campo estado emocional predominante es obligatorio")]
        public string predominantEmotionalState { get; set; } = null!;

        public Account? account { get; set; }
    }
}
