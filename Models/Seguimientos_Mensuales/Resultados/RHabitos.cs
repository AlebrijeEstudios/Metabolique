using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Seguimientos_Mensuales.Resultados
{
    public class RHabitos
    {
        [Key]
        public Guid resultadosID { get; set; } = Guid.NewGuid();

        [ForeignKey("SegMenHabitos")]
        public Guid seguimientoMensualID { get; set; }

        [Required(ErrorMessage = "El campo resultado del componente 1 es obligatorio")]
        public int resultadoComponente1 { get; set; }

        [Required(ErrorMessage = "El campo resultado del componente 2 es obligatorio")]
        public int resultadoComponente2 { get; set; }

        [Required(ErrorMessage = "El campo resultado del componente 3 es obligatorio")]
        public int resultadoComponente3 { get; set; }

        [Required(ErrorMessage = "El campo resultado del componente 4 es obligatorio")]
        public int resultadoComponente4 { get; set; }

        [Required(ErrorMessage = "El campo resultado del componente 5 es obligatorio")]
        public int resultadoComponente5 { get; set; }

        [Required(ErrorMessage = "El campo resultado del componente 6 es obligatorio")]
        public int resultadoComponente6 { get; set; }

        [Required(ErrorMessage = "El campo resultado del componente 7 es obligatorio")]
        public int resultadoComponente7 { get; set; }

        [Required(ErrorMessage = "El campo clasificacion global es obligatorio")]
        public int clasificacionGlobal { get; set; }

        [Required(ErrorMessage = "El campo clasificacion es obligatorio")]
        public string clasificacion { get; set; } = null!;

        public SegMenHabitos? seguimientoMensualHabitos { get; set; }

    }
}
