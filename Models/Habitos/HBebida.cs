using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Habitos
{
    public class HBebida
    {
        [Key]
        public Guid habitoID { get; set; } = Guid.NewGuid();

        [ForeignKey("Cuenta")]
        public Guid cuentaID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        [DataType(DataType.Date)]
        public DateOnly fecha { get; set; }

        [Required(ErrorMessage = "El campo tipo de bebida es obligatorio")]
        public string tipoBebida { get; set; } = null!;

        [Required(ErrorMessage = "El campo cantidad consumida es obligatorio")]
        public string cantidadConsumida { get; set; } = null!;

        public Cuenta? cuenta { get; set; }
    }
}
