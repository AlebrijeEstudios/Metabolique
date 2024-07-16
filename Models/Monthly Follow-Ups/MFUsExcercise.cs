using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Seguimientos_Mensuales
{
   
    public class MFUsExcercise
    {
        [Key]
        public Guid monthlyFollowUpID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo mes es obligatorio")]
        public string month { get; set; } = null!;

        [Required(ErrorMessage = "El campo año es obligatorio")]
        public int year { get; set; }

        [Required(ErrorMessage = "El campo pregunta1 es obligatorio")]
        public int question1 { get; set; }

        [Required(ErrorMessage = "El campo pregunta2 es obligatorio")]
        public int question2 { get; set; }

        [Required(ErrorMessage = "El campo pregunta3 es obligatorio")]
        public int question3 { get; set; }

        [Required(ErrorMessage = "El campo pregunta4 es obligatorio")]
        public int question4 { get; set; }

        [Required(ErrorMessage = "El campo pregunta5 es obligatorio")]
        public int question5 { get; set; }

        [Required(ErrorMessage = "El campo pregunta6 es obligatorio")]
        public int question6 { get; set; }

        [Required(ErrorMessage = "El campo pregunta7 es obligatorio")]
        public int question7 { get; set; }

        [Required(ErrorMessage = "El campo actividad caminata es obligatorio")]
        public float actWalking { get; set; }

        [Required(ErrorMessage = "El campo actividad fisica moderada es obligatorio")]
        public float actModerate { get; set; }

        [Required(ErrorMessage = "El campo actividad fisica vigorosa es obligatorio")]
        public float actVigorous { get; set; }

        [Required(ErrorMessage = "El campo totalMET es obligatorio")]
        public float totalMET { get; set; }

        [Required(ErrorMessage = "El campo conducta sendentaria es obligatorio")]
        public string sedentaryBehavior { get; set; } = null!;

        [Required(ErrorMessage = "El campo nivelAF es obligatorio")]
        public string levelAF { get; set; } = null!;

        public Account? account { get; set; }

    }
}
