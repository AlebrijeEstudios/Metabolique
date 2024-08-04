using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Medications
{
    public class Medication
    {
        [Key]
        public Guid medicationID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo nombre del medicamento es obligatorio.")]
        public string nameMedication { get; set; } = null!;

        [Required(ErrorMessage = "El campo dosis es obligatorio.")]
        public int dose { get; set; }

        [Required(ErrorMessage = "El campo frecuencia inicial es obligatorio.")]
        public DateOnly initialFrec { get; set; } 

        [Required(ErrorMessage = "El campo frecuencia final es obligatorio.")]
        public DateOnly finalFrec { get; set; }

        [Required(ErrorMessage = "El campo dosis es obligatorio.")]
        public int dailyFrec { get; set; }

        public Account? account { get; set; }

        public ICollection<Times> times { get; set; } = new List<Times>();

    }
}
