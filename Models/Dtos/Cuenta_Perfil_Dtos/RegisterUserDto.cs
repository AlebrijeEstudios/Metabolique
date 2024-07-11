using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos
{
    public class RegisterUserDto
    {
        public string username { get; set; } = null!;

        public string email { get; set; } = null!;

        public string password { get; set; } = null!;

    }
}
