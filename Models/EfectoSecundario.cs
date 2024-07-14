using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models
{
    public class EfectoSecundario
    {
        [Key]
        public Guid registroID { get; set; } = Guid.NewGuid();

        [ForeignKey("Cuenta")]
        public Guid cuentaID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        [DataType(DataType.Date)]
        public DateOnly fecha { get; set; }

        [Required(ErrorMessage = "El campo horario inicial es obligatorio")]
        [DataType(DataType.Time)]
        public TimeOnly horarioInicio { get; set; }

        [Required(ErrorMessage = "El campo horario final es obligatorio")]
        [DataType(DataType.Time)]
        public TimeOnly horarioFinal { get; set; }

        [Required(ErrorMessage = "El campo descripcion es obligatorio")]
        public string descripcion { get; set; } = null!;

        public Cuenta? cuenta { get; set; }

    }
}
