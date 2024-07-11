using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos
{
    public class LoginUserDto
    {
        public string email { get; set; } = null!;

        public string password { get; set; } = null!;

    }
}
