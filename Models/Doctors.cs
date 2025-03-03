using AppVidaSana.Models.Feeding;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppVidaSana.Models
{
    public class Doctors
    {
        [Key]
        public Guid doctorID { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El campo username es obligatorio.")]
        public string username { get; set; } = null!;

        [Required(ErrorMessage = "El campo email es obligatorio.")]
        [EmailAddress(ErrorMessage = "Debe tener la estructura de un correo.")]
        public string email { get; set; } = null!;

        [Required(ErrorMessage = "El campo contraseña es obligatoria.")]
        public string password { get; set; } = null!;

        [ForeignKey("Roles")]
        public Guid roleID { get; set; }

        public Roles? roles { get; set; }

        public ICollection<PacientDoctor> pacientDoctor{ get; set; } = new List<PacientDoctor>();
    }
}
