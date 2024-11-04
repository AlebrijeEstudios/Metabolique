using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Exercises
{
    public class Exercise
    {
        [Key]
        public Guid exerciseID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatoria.")]
        public DateOnly dateExercise { get; set; }

        [Required(ErrorMessage = "El campo tipo es obligatorio.")]
        public string typeExercise { get; set; } = null!;

        [Required(ErrorMessage = "El campo intensidad es obligatorio.")]
        public string intensityExercise { get; set; } = null!;

        [Required(ErrorMessage = "El campo tiempo es obligatorio.")]
        public int timeSpent { get; set; }

        public Account? account { get; set; }

    }
}
