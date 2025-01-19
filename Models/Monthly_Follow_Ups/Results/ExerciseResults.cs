using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Monthly_Follow_Ups.Results
{
    public class ExerciseResults
    {
        [Key]
        public Guid resultsID { get; set; } = Guid.NewGuid();

        [ForeignKey("MFUsExercise")]
        public Guid monthlyFollowUpID { get; set; }

        [Required(ErrorMessage = "El campo actividad caminata es obligatorio.")]
        public float actWalking { get; set; }

        [Required(ErrorMessage = "El campo actividad física moderada es obligatorio.")]
        public float actModerate { get; set; }

        [Required(ErrorMessage = "El campo actividad física vigorosa es obligatorio.")]
        public float actVigorous { get; set; }

        [Required(ErrorMessage = "El campo totalMET es obligatorio.")]
        public float totalMET { get; set; }

        [Required(ErrorMessage = "El campo conducta sedentaria es obligatorio.")]
        public string sedentaryBehavior { get; set; } = null!;

        [Required(ErrorMessage = "El campo nivelAF es obligatorio.")]
        public string levelAF { get; set; } = null!;

        public MFUsExercise? MFUsExercise { get; set; }
    }
}
