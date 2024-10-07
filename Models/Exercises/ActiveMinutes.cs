using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Exercises
{
    public class ActiveMinutes
    {
        [Key]
        public Guid timeSpentID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatoria")]
        public DateOnly dateExercise { get; set; }

        [Required(ErrorMessage = "El campo tiempo total es obligatorio")]
        public int totalTimeSpent { get; set; }

        public Account? account { get; set; }

    }
}
