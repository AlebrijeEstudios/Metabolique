using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Habitos
{
    public class SleepHabit
    {
        [Key]
        public Guid sleepHabitID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        public DateOnly sleepDateHabit { get; set; }

        [Required(ErrorMessage = "El campo horas de sueño es obligatorio")]
        public int sleepHours { get; set; }

        [Required(ErrorMessage = "El campo percepción de descanso es obligatorio")]
        public string perceptionOfRelaxation { get; set; } = null!;

        public Account? account { get; set; }

    }
}
