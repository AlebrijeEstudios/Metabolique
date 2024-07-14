using AppVidaSana.Models.Alimentación;
using AppVidaSana.Models.Habitos;
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

        public ICollection<Desayuno> desayunos { get; set; } = new List<Desayuno>();

        public ICollection<Almuerzo> almuerzos { get; set; } = new List<Almuerzo>();

        public ICollection<Comida> comidas { get; set; } = new List<Comida>();

        public ICollection<Colacion> colaciones { get; set; } = new List<Colacion>();

        public ICollection<Cena> cenas { get; set; } = new List<Cena>();

        public ICollection<SegMenAlimentacion> segMenAlimentacion { get; set; } = new List<SegMenAlimentacion>();

        public ICollection<Ejercicio> ejercicios { get; set; } = new List<Ejercicio>();

        public ICollection<SegMenEjercicio> segMenEjercicio { get; set; } = new List<SegMenEjercicio>();

        public ICollection<Medicamento> medicamentos { get; set; } = new List<Medicamento>();
        
        public ICollection<EfectoSecundario> efectoSecundarios { get; set; } = new List<EfectoSecundario>();

        public ICollection<SegMenMedicamentos> segMenMedicamentos { get; set; } = new List<SegMenMedicamentos>();

        public ICollection<HBebida> habitosBebida { get; set; } = new List<HBebida>();

        public ICollection<HDrogas> habitosDroga { get; set; } = new List<HDrogas>();

        public ICollection<HSueño> habitosSueño { get; set; } = new List<HSueño>();

        public ICollection<SegMenHabitos> segMenHabitos { get; set; } = new List<SegMenHabitos>();

    }
}
