using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Seguimientos_Mensuales.Respuestas
{
    public class RAlimentacion
    {
        [Key]
        public Guid resultadosID { get; set; } = Guid.NewGuid();

        [ForeignKey("SegMenAlimentacion")]
        public Guid seguimientoMensualID { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 1 es obligatorio")]
        public int puntosPregunta1 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 2 es obligatorio")]
        public int puntosPregunta2 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 3 es obligatorio")]
        public int puntosPregunta3 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 4 es obligatorio")]
        public int puntosPregunta4 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 5 es obligatorio")]
        public int puntosPregunta5 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 6 es obligatorio")]
        public int puntosPregunta6 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 7 es obligatorio")]
        public int puntosPregunta7 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 8 es obligatorio")]
        public int puntosPregunta8 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 9 es obligatorio")]
        public int puntosPregunta9 { get; set; }

        [Required(ErrorMessage = "El campo puntos de la pregunta 10 es obligatorio")]
        public int puntosPregunta10 { get; set; }

        [Required(ErrorMessage = "El campo total de puntos es obligatorio")]
        public int totalPuntos { get; set; }

        [Required(ErrorMessage = "El campo clasificacion es obligatorio")]
        public string clasificacion { get; set; } = null!;

        public SegMenAlimentacion? seguimientoMensualAlimentos { get; set; }

    }
}
