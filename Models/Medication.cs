using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models
{
    public class Medication
    {
        [Key]
        public Guid medicationID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        [DataType(DataType.Date)]
        public DateOnly dateMedication { get; set; }

        [Required(ErrorMessage = "El campo nombre del medicamento es obligatorio")]
        public string nameMedication { get; set; } = null!;

        [Required(ErrorMessage = "El campo dosis es obligatorio")]
        public int dose { get; set; }

        [Required(ErrorMessage = "El campo frecuencia semanal es obligatorio")]
        public string weeklyFrequency { get; set; } = null!;

        [Required(ErrorMessage = "El campo frecuencia diaria es obligatorio")]
        public int dailyFrequency { get; set; }

        [Required(ErrorMessage = "El campo horario 1 es obligatorio")]
        [DataType(DataType.Time)]
        public TimeOnly schedule1 { get; set; }

        [Required(ErrorMessage = "El campo horario 2 es obligatorio")]
        [DataType(DataType.Time)]
        public TimeOnly schedule2 { get; set; }

        [Required(ErrorMessage = "El campo horario 3 es obligatorio")]
        [DataType(DataType.Time)]
        public TimeOnly schedule3 { get; set; }

        public Account? account { get; set; }

    }
}
