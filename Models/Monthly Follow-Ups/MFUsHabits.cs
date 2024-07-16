using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppVidaSana.Models.Seguimientos_Mensuales.Resultados;

namespace AppVidaSana.Models.Seguimientos_Mensuales
{
    public class MFUsHabits
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
        [DataType(DataType.Time)]
        public TimeOnly answerQuestion1 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 2 es obligatorio")]
        public int answerQuestion2 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 3 es obligatoria")]
        [DataType(DataType.Time)]
        public TimeOnly answerQuestion3 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 4 es obligatorio")]
        public int answerQuestion4 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5a es obligatorio")]
        public string answerQuestion5a { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5b es obligatorio")]
        public string answerQuestion5b { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5c es obligatorio")]
        public string answerQuestion5c { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5d es obligatorio")]
        public string answerQuestion5d { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5e es obligatorio")]
        public string answerQuestion5e { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5f es obligatorio")]
        public string answerQuestion5f { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5g es obligatorio")]
        public string answerQuestion5g { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5h es obligatorio")]
        public string answerQuestion5h { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5i es obligatorio")]
        public string answerQuestion5i { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5j es obligatorio")]
        public string answerQuestion5j { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 6 es obligatorio")]
        public string answerQuestion6 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 7 es obligatorio")]
        public string answerQuestion7 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 8 es obligatorio")]
        public string answerQuestion8 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 9 es obligatorio")]
        public string answerQuestion9 { get; set; } = null!;

        public Account? account { get; set; }

        public HabitsResults? results { get; set; }
    }
}
