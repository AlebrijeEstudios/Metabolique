using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Habitos
{
    public class HSueño
    {
        [Key]
        public Guid habitoID { get; set; } = Guid.NewGuid();

        [ForeignKey("Cuenta")]
        public Guid cuentaID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        [DataType(DataType.Date)]
        public DateOnly fecha { get; set; }

        [Required(ErrorMessage = "El campo horas de sueño es obligatorio")]
        public int horasSueño { get; set; }

        [Required(ErrorMessage = "El campo percepcion de descanso es obligatorio")]
        public string percepcionDescanso { get; set; } = null!;

        public Cuenta? cuenta { get; set; }

    }
}
