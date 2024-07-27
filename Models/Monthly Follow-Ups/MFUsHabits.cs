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
        public TimeOnly answerQuestion1 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 2 es obligatorio")]
        public int answerQuestion2 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 3 es obligatoria")]
        public TimeOnly answerQuestion3 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 4 es obligatorio")]
        public int answerQuestion4 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5a es obligatorio")]
        public int answerQuestion5a { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5b es obligatorio")]
        public int answerQuestion5b { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5c es obligatorio")]
        public int answerQuestion5c { get; set; } 

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5d es obligatorio")]
        public int answerQuestion5d { get; set; } 

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5e es obligatorio")]
        public int answerQuestion5e { get; set; } 

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5f es obligatorio")]
        public int answerQuestion5f { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5g es obligatorio")]
        public int answerQuestion5g { get; set; } 

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5h es obligatorio")]
        public int answerQuestion5h { get; set; } 

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5i es obligatorio")]
        public int answerQuestion5i { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5j es obligatorio")]
        public int answerQuestion5j { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 6 es obligatorio")]
        public int answerQuestion6 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 7 es obligatorio")]
        public int answerQuestion7 { get; set; } 

        [Required(ErrorMessage = "El campo respuesta de la pregunta 8 es obligatorio")]
        public int answerQuestion8 { get; set; } 

        [Required(ErrorMessage = "El campo respuesta de la pregunta 9 es obligatorio")]
        public int answerQuestion9 { get; set; } 

        public Account? account { get; set; }

        public HabitsResults? results { get; set; }
    }
}
