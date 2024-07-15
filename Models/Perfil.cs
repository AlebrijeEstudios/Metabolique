using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Models
{
    [PrimaryKey(nameof(cuentaID))]
    public class Perfil
    {
        [ForeignKey("Cuenta")]
        public Guid cuentaID { get; set; }

        [Required(ErrorMessage = "El campo fecha de nacimiento es obligatoria")]
        [DataType(DataType.Date)]
        public DateOnly fechaNacimiento { get; set; }

        [Required(ErrorMessage = "El campo sexo es obligatorio")]
        public string sexo { get; set; } = null!;

        [Required(ErrorMessage = "El campo estatura es obligatorio")]
        public int estatura { get; set; }

        [Required(ErrorMessage = "El campo peso es obligatorio")]
        public int peso { get; set; }

        [Required(ErrorMessage = "El campo protocolo es obligatorio")]
        public string protocolo { get; set; } = null!;

        public Cuenta? cuenta { get; set; }

    }
}
