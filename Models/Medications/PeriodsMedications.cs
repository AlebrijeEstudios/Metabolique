﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Medications
{
    public class PeriodsMedications
    {
        [Key]
        public Guid periodID { get; set; } = Guid.NewGuid();

        [ForeignKey("Medication")]
        public Guid medicationID { get; set; }

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo frecuencia inicial es obligatorio.")]
        public DateOnly initialFrec { get; set; }

        [Required(ErrorMessage = "El campo frecuencia final es obligatorio.")]
        public DateOnly finalFrec { get; set; }

        [Required(ErrorMessage = "El campo dosis es obligatorio.")]
        public string dose { get; set; } = null!;

        [Required(ErrorMessage = "El campo tiempos del periodo es obligatorio")]
        public string timesPeriod { get; set; } = null!;

        public string? datesExcluded { get; set; }

        public Medication? medication { get; set; }

        public Account? account { get; set; }

        public ICollection<DaysConsumedOfMedications> daysConsumedOfMedications { get; set; } = new List<DaysConsumedOfMedications>();
    }
}
