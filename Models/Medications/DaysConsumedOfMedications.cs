using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Medications
{
    public class DaysConsumedOfMedications
    {
        [Key]
        public Guid dayConsumedID { get; set; } = Guid.NewGuid();

        [ForeignKey("PeriodsMedications")]
        public Guid periodID { get; set; }

        [Required(ErrorMessage = "El campo fecha de consumo es obligatorio.")]
        public DateOnly dateConsumed { get; set; }

        public string? consumptionTimes { get; set; }

        public PeriodsMedications? periodMedication { get; set; }

        public ICollection<Times> times { get; set; } = new List<Times>();
    }
}
