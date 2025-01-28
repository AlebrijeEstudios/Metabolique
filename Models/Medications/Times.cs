using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Medications
{
    public class Times
    {
        [Key]
        public Guid timeID { get; set; } = Guid.NewGuid();

        [ForeignKey("DaysConsumedOfMedications")]
        public Guid dayConsumedID { get; set; }

        [Required(ErrorMessage = "El campo horario es obligatoria.")]
        public TimeOnly time { get; set; }

        [Required(ErrorMessage = "El campo status de medicamento es obligatorio.")]
        public bool medicationStatus { get; set; }

        public DaysConsumedOfMedications? daysConsumedOfMedications { get; set; }
    }
}
