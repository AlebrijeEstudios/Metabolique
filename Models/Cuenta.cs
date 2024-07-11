using AppVidaSana.Models.Seguimientos_Mensuales;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models
{
    public class Cuenta
    {
        [Key]
        public Guid id { get; set; } = Guid.NewGuid();

        [Required(ErrorMessage = "El campo username es obligatorio")]
        public string username { get; set; } = null!;

        [Required(ErrorMessage = "El campo email es obligatorio")]
        [EmailAddress(ErrorMessage = "Debe tener la estructura de un correo.")]
        public string email { get; set; } = null!;

        [Required(ErrorMessage = "El campo contraseña es obligatoria")]
        public string password { get; set; } = null!;

        public string role { get; set; } = "User";

        public Perfil? perfil { get; set; }

        public ICollection<Ejercicio> ejercicios { get; set; } = new List<Ejercicio>();

        public SegMenEjercicio? seg_men_ej { get; set; }

    }
}
