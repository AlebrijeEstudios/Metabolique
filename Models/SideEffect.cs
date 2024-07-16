using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models
{
    public class SideEffect
    {
        [Key]
        public Guid sideEffectID { get; set; } = Guid.NewGuid();

        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        [DataType(DataType.Date)]
        public DateOnly dateSideEffect { get; set; }

        [Required(ErrorMessage = "El campo horario inicial es obligatorio")]
        [DataType(DataType.Time)]
        public TimeOnly initialSchedule { get; set; }

        [Required(ErrorMessage = "El campo horario final es obligatorio")]
        [DataType(DataType.Time)]
        public TimeOnly endSchedule { get; set; }

        [Required(ErrorMessage = "El campo descripcion es obligatorio")]
        public string description { get; set; } = null!;

        public Account? account { get; set; }

    }
}
