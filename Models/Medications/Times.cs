using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Medications
{
    public class Times
    {
        [Key]
        public Guid timeID { get; set; } = Guid.NewGuid();

        [ForeignKey("Medication")]
        public Guid medicationID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatoria.")]
        public DateOnly dateMedication { get; set; }

        [Required(ErrorMessage = "El campo horario es obligatoria.")]
        public TimeOnly time { get; set; } 

        [Required(ErrorMessage = "El campo status de medicamento es obligatorio.")]
        public bool medicationStatus { get; set; }

        public Medication? medication { get; set; }
    }
}
