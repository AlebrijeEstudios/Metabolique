using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Habits
{
    public class SleepHabit
    {
        [Key]
        public Guid sleepHabitID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        public DateOnly sleepDateHabit { get; set; }

        public int? sleepHours  { get; set; }

        public string? perceptionOfRelaxation  { get; set; }

        public Account? account { get; set; }

    }
}
