using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppVidaSana.Models.Alimentación.Alimentos;

namespace AppVidaSana.Models.Alimentación
{
    public class Comida
    {
        [Key]
        public Guid comidaID { get; set; } = Guid.NewGuid();

        [ForeignKey("Cuenta")]
        public Guid cuentaID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        [DataType(DataType.Date)]
        public DateOnly fecha { get; set; }

        [Required(ErrorMessage = "El campo hora es obligatorio")]
        [DataType(DataType.Time)]
        public TimeOnly hora { get; set; }

        [Required(ErrorMessage = "El campo nivel de saciedad es obligatorio")]
        public string nivelSaciedad { get; set; } = null!;

        [Required(ErrorMessage = "El campo emociones ligadas es obligatorio")]
        public string emocionesLigadas { get; set; } = null!;

        public Cuenta? cuenta { get; set; }

        public ICollection<Alimentos_Comida> alimentosComida { get; set; } = new List<Alimentos_Comida>();
    }
}
