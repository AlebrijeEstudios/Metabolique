using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Monthly_Follow_Ups
{
    public class MFUsMedication
    {
        [Key]
        public Guid monthlyFollowUpID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [ForeignKey("MFUsMonths")]
        public Guid monthID { get; set; }

        [Required(ErrorMessage = "El campo respuesta de pregunta 1 es obligatorio")]
        public bool answerQuestion1 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de pregunta 2 es obligatorio")]
        public bool answerQuestion2 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de pregunta 3 es obligatorio")]
        public bool answerQuestion3 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de pregunta 4 es obligatorio")]
        public bool answerQuestion4 { get; set; }

        [Required(ErrorMessage = "El campo status de adherencia es obligatorio")]
        public bool statusAdherence { get; set; }

        public Account? account { get; set; }

        public MFUsMonths? months { get; set; }

    }
}
