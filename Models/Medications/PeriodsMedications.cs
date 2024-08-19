using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Medications
{
    public class PeriodsMedications
    {
        [Key]
        public Guid periodID { get; set; } = Guid.NewGuid();

        [ForeignKey("Medication")]
        public Guid medicationID { get; set; }

        [Required(ErrorMessage = "El campo frecuencia inicial es obligatorio.")]
        public DateOnly initialFrec { get; set; }

        [Required(ErrorMessage = "El campo frecuencia final es obligatorio.")]
        public DateOnly finalFrec { get; set; }

        public bool isActive { get; set; }

        public Medication? medication { get; set; }

        public ICollection<Times> times { get; set; } = new List<Times>();

    }
}
