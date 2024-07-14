using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppVidaSana.Models.Seguimientos_Mensuales.Respuestas;

namespace AppVidaSana.Models.Seguimientos_Mensuales
{
    public class SegMenAlimentacion
    {
        [Key]
        public Guid seguimientoMensualID { get; set; } = Guid.NewGuid();

        [ForeignKey("Cuenta")]
        public Guid cuentaID { get; set; }

        [Required(ErrorMessage = "El campo mes es obligatorio")]
        public string mes { get; set; } = null!;

        [Required(ErrorMessage = "El campo año es obligatorio")]
        public int año { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 1 es obligatorio")]
        public string respuestaPregunta1 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 2 es obligatorio")]
        public string respuestaPregunta2 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 3 es obligatorio")]
        public string respuestaPregunta3 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 4 es obligatorio")]
        public string respuestaPregunta4 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5 es obligatorio")]
        public string respuestaPregunta5 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 6 es obligatorio")]
        public string respuestaPregunta6 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 7 es obligatorio")]
        public string respuestaPregunta7 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 8 es obligatorio")]
        public string respuestaPregunta8 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 9 es obligatorio")]
        public string respuestaPregunta9 { get; set; } = null!;

        public Cuenta? cuenta { get; set; }

        public RAlimentacion? resultados { get; set; }

    }
}
