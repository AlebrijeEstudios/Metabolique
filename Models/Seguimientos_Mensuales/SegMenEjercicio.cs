using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models.Seguimientos_Mensuales
{
   
    public class SegMenEjercicio
    {
        [Key]
        public Guid seguimientoMensualID { get; set; } = Guid.NewGuid();

        [ForeignKey("Cuenta")]
        public Guid cuentaID { get; set; }

        [Required(ErrorMessage = "El campo mes es obligatorio")]
        public string mes { get; set; } = null!;

        [Required(ErrorMessage = "El campo año es obligatorio")]
        public int año { get; set; }

        [Required(ErrorMessage = "El campo pregunta1 es obligatorio")]
        public int pregunta1 { get; set; }

        [Required(ErrorMessage = "El campo pregunta2 es obligatorio")]
        public int pregunta2 { get; set; }

        [Required(ErrorMessage = "El campo pregunta3 es obligatorio")]
        public int pregunta3 { get; set; }

        [Required(ErrorMessage = "El campo pregunta4 es obligatorio")]
        public int pregunta4 { get; set; }

        [Required(ErrorMessage = "El campo pregunta5 es obligatorio")]
        public int pregunta5 { get; set; }

        [Required(ErrorMessage = "El campo pregunta6 es obligatorio")]
        public int pregunta6 { get; set; }

        [Required(ErrorMessage = "El campo pregunta7 es obligatorio")]
        public int pregunta7 { get; set; }

        [Required(ErrorMessage = "El campo actcaminata es obligatorio")]
        public float actCaminata { get; set; }

        [Required(ErrorMessage = "El campo afmoderada es obligatorio")]
        public float actfModerada { get; set; }

        [Required(ErrorMessage = "El campo afvigorosa es obligatorio")]
        public float actfVigorosa { get; set; }

        [Required(ErrorMessage = "El campo totalMET es obligatorio")]
        public float totalMET { get; set; }

        [Required(ErrorMessage = "El campo conductasend es obligatorio")]
        public string conductaSend { get; set; } = null!;

        [Required(ErrorMessage = "El campo nivelAF es obligatorio")]
        public string nivelAF { get; set; } = null!;

        public Cuenta? cuenta { get; set; }

    }
}
