using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Habits
{
    public class DrugsHabit
    {
        [Key]
        public Guid drugsHabitID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        public DateOnly drugsDateHabit { get; set; }

        public int? cigarettesSmoked  { get; set; }

        public string? predominantEmotionalState  { get; set; }

        public Account? account { get; set; }
    }
}
