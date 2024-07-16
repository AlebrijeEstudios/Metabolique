namespace AppVidaSana.Exceptions.Cuenta_Perfil
{
    public class LoginException : Exception
    {
        public LoginException() : base("El usuario o contraseña no existen y/o estan incorrectos.")
        {
        }
    }
}
