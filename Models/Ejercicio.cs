using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models
{
    public class Ejercicio
    {
        [Key]
        public Guid idejercicio { get; set; } = Guid.NewGuid();

        [ForeignKey("Cuenta")]
        public Guid id { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatoria")]
        [DataType(DataType.Date)]
        public DateOnly fecha { get; set; }

        [Required(ErrorMessage = "El campo tipo es obligatorio")]
        public string tipo { get; set; } = null!;

        [Required(ErrorMessage = "El campo intensidad es obligatorio")]
        public string intensidad { get; set; } = null!;

        [Required(ErrorMessage = "El campo tiempo es obligatorio")]
        public int tiempo { get; set; }

        public Cuenta? cuenta { get; set; }

    }
}
