using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models
{
    public class Medicamento
    {
        [Key]
        public Guid medicamentoID { get; set; } = Guid.NewGuid();

        [ForeignKey("Cuenta")]
        public Guid cuentaID { get; set; }

        [Required(ErrorMessage = "El campo fecha es obligatorio")]
        [DataType(DataType.Date)]
        public DateOnly fecha { get; set; }

        [Required(ErrorMessage = "El campo nombre del medicamento es obligatorio")]
        public string nombreMedicamento { get; set; } = null!;

        [Required(ErrorMessage = "El campo dosis es obligatorio")]
        public int dosis { get; set; }

        [Required(ErrorMessage = "El campo frecuencia semanal es obligatorio")]
        public string frecuenciaSem { get; set; } = null!;

        [Required(ErrorMessage = "El campo frecuencia diaria es obligatorio")]
        public int frecuenciaDiaria { get; set; }

        [Required(ErrorMessage = "El campo horario 1 es obligatorio")]
        [DataType(DataType.Time)]
        public TimeOnly horario1 { get; set; }

        [Required(ErrorMessage = "El campo horario 2 es obligatorio")]
        [DataType(DataType.Time)]
        public TimeOnly horario2 { get; set; }

        [Required(ErrorMessage = "El campo horario 3 es obligatorio")]
        [DataType(DataType.Time)]
        public TimeOnly horario3 { get; set; }

        public Cuenta? cuenta { get; set; }

    }
}
