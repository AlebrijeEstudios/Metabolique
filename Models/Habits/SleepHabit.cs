using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Habitos
{
    public class SleepHabit
    {
        [Key]
        public Guid habitID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        [DataType(DataType.Date)]
        public DateOnly dateHabit { get; set; }

        [Required(ErrorMessage = "El campo horas de sueño es obligatorio")]
        public int sleepHours { get; set; }

        [Required(ErrorMessage = "El campo percepcion de descanso es obligatorio")]
        public string perceptionOfRelaxation { get; set; } = null!;

        public Account? account { get; set; }

    }
}
