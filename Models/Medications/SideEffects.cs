using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Medications
{
    public class SideEffects
    {
        [Key]
        public Guid sideEffectID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatoria.")]
        public DateOnly dateSideEffects { get; set; }

        [Required(ErrorMessage = "El campo horario inicial es obligatoria.")]
        public TimeOnly initialTime { get; set; }

        [Required(ErrorMessage = "El campo horario final es obligatoria.")]
        public TimeOnly finalTime { get; set; }

        [Required(ErrorMessage = "El campo descripción es obligatoria.")]
        [MaxLength(255, ErrorMessage = "El máximo número de caráteres es de 255.")]
        public string description { get; set; } = null!;

        public Account? account { get; set; }

    }
}
