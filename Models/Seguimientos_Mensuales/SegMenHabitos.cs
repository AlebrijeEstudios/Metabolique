using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AppVidaSana.Models.Seguimientos_Mensuales.Resultados;

namespace AppVidaSana.Models.Seguimientos_Mensuales
{
    public class SegMenHabitos
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
        [DataType(DataType.Time)]
        public TimeOnly respuestaPregunta1 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 2 es obligatorio")]
        public int respuestaPregunta2 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 3 es obligatoria")]
        [DataType(DataType.Time)]
        public TimeOnly respuestaPregunta3 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 4 es obligatorio")]
        public int respuestaPregunta4 { get; set; }

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5a es obligatorio")]
        public string respuestaPregunta5a { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5b es obligatorio")]
        public string respuestaPregunta5b { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5c es obligatorio")]
        public string respuestaPregunta5c { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5d es obligatorio")]
        public string respuestaPregunta5d { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5e es obligatorio")]
        public string respuestaPregunta5e { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5f es obligatorio")]
        public string respuestaPregunta5f { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5g es obligatorio")]
        public string respuestaPregunta5g { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5h es obligatorio")]
        public string respuestaPregunta5h { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5i es obligatorio")]
        public string respuestaPregunta5i { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 5j es obligatorio")]
        public string respuestaPregunta5j { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 6 es obligatorio")]
        public string respuestaPregunta6 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 7 es obligatorio")]
        public string respuestaPregunta7 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 8 es obligatorio")]
        public string respuestaPregunta8 { get; set; } = null!;

        [Required(ErrorMessage = "El campo respuesta de la pregunta 9 es obligatorio")]
        public string respuestaPregunta9 { get; set; } = null!;

        public Cuenta? cuenta { get; set; }

        public RHabitos? resultados { get; set; }
    }
}
