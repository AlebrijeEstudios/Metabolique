using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Medications
{
    public class Medication
    {
        [Key]
        public Guid medicationID { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El campo nombre del medicamento es obligatorio")]
        public string nameMedication { get; set; } = null!;

        public ICollection<PeriodsMedications> periods { get; set; } = new List<PeriodsMedications>();

    }
}
