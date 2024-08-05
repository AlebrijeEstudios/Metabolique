using AppVidaSana.Models.Monthly_Follow_Ups;
using AppVidaSana.Models.Monthly_Follow_Ups.Results;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Seguimientos_Mensuales
{
    public class MFUsExercise
    {
        [Key]
        public Guid monthlyFollowUpID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [ForeignKey("MFUsMonths")]
        public Guid monthID { get; set; }

        [Required(ErrorMessage = "El campo pregunta1 es obligatorio.")]
        public int question1 { get; set; }

        [Required(ErrorMessage = "El campo pregunta2 es obligatorio.")]
        public int question2 { get; set; }

        [Required(ErrorMessage = "El campo pregunta3 es obligatorio.")]
        public int question3 { get; set; }

        [Required(ErrorMessage = "El campo pregunta4 es obligatorio.")]
        public int question4 { get; set; }

        [Required(ErrorMessage = "El campo pregunta5 es obligatorio.")]
        public int question5 { get; set; }

        [Required(ErrorMessage = "El campo pregunta6 es obligatorio.")]
        public int question6 { get; set; }

        [Required(ErrorMessage = "El campo pregunta7 es obligatorio.")]
        public int question7 { get; set; }

        public Account? account { get; set; }

        public ExerciseResults? results { get; set; }

        public MFUsMonths? months { get; set; }

    }
}
