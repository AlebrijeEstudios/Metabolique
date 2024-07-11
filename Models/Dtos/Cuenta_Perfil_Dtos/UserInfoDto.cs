using Newtonsoft.Json;

namespace AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos
{
    public class UserInfoDto
    {
        public Guid id { get; set; }

        public string username { get; set; } = null!;

        public string email { get; set; } = null!;

    }
}
