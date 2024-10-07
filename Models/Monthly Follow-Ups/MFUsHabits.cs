using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Models.Seguimientos_Mensuales.Resultados;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Seguimientos_Mensuales
{
    public class MFUsHabits
    {
        [Key]
        public Guid monthlyFollowUpID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [ForeignKey("MFUsMonths")]
        public Guid monthID { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 1 es obligatorio")]
        public TimeOnly answerQuestion1 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 2 es obligatorio")]
        public byte answerQuestion2 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 3 es obligatoria")]
        public TimeOnly answerQuestion3 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 4 es obligatorio")]
        public int answerQuestion4 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5a es obligatorio")]
        public byte answerQuestion5a { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5b es obligatorio")]
        public byte answerQuestion5b { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5c es obligatorio")]
        public byte answerQuestion5c { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5d es obligatorio")]
        public byte answerQuestion5d { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5e es obligatorio")]
        public byte answerQuestion5e { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5f es obligatorio")]
        public byte answerQuestion5f { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5g es obligatorio")]
        public byte answerQuestion5g { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5h es obligatorio")]
        public byte answerQuestion5h { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5i es obligatorio")]
        public byte answerQuestion5i { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5j es obligatorio")]
        public byte answerQuestion5j { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 6 es obligatorio")]
        public byte answerQuestion6 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 7 es obligatorio")]
        public byte answerQuestion7 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 8 es obligatorio")]
        public byte answerQuestion8 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 9 es obligatorio")]
        public byte answerQuestion9 { get; set; }

        public Account? account { get; set; }

        public HabitsResults? results { get; set; }

        public MFUsMonths? months { get; set; }
    }
}
