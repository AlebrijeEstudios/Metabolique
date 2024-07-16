using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppVidaSana.Models.Seguimientos_Mensuales.Respuestas;

namespace AppVidaSana.Models.Seguimientos_Mensuales
{
    public class MFUsMedications
    {
        [Key]
        public Guid monthlyFollowUpID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo mes es obligatorio")]
        public string month { get; set; } = null!;

        [Required(ErrorMessage = "El campo año es obligatorio")]
        public int year { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 1 es obligatorio")]
        public string answerQuestion1 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 2 es obligatorio")]
        public string answerQuestion2 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 3 es obligatorio")]
        public string answerQuestion3 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 4 es obligatorio")]
        public string answerQuestion4 { get; set; } = null!;

        [Required(ErrorMessage = "El campo adherencia al tratamiento es obligatorio")]
        public string treatmentAdherence { get; set; } = null!;

        public Account? account { get; set; }

    }
}
