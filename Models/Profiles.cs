using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AppVidaSana.Models
{
    [PrimaryKey(nameof(accountID))]
    public class Profiles
    {
        [ForeignKey("Account")]
        public Guid accountID { get; set; }

        [Required(ErrorMessage = "El campo fecha de nacimiento es obligatoria.")]
        public DateOnly birthDate { get; set; }

        [Required(ErrorMessage = "El campo sexo es obligatorio.")]
        public string sex { get; set; } = null!;

        [Required(ErrorMessage = "El campo estatura es obligatorio.")]
        public float stature { get; set; }

        [Required(ErrorMessage = "El campo peso es obligatorio.")]
        public float weight { get; set; }

        [Required(ErrorMessage = "El campo protocolo es obligatorio.")]
        public string protocolToFollow { get; set; } = null!;

        public Account? account { get; set; }

    }
}
