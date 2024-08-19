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

        public Account? account { get; set; }

        public ICollection<PeriodsMedications> periods { get; set; } = new List<PeriodsMedications>();

    }
}
