using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos
{
    public class ProfileUserDto
    {
        public Guid id { get; set; }

        public DateOnly fechaNacimiento { get; set; }

        public string sexo { get; set; } = null!;

        public int estatura { get; set; }

        public int peso { get; set; }

        public string protocolo { get; set; } = null!;
    }
}
