using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos
{
    public class ResetPasswordDto
    {
        public string email { get; set; } = null!;

        public string token { get; set; } = null!;

        public string password { get; set; } = null!;

        public string confirmpassword { get; set; } = null!;

    }
}
