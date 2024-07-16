namespace AppVidaSana.Exceptions.Cuenta_Perfil
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base("Usuario no encontrado.")
        {
        }
    }
}
