namespace AppVidaSana.Exceptions.Cuenta_Perfil
{
    public class FailLoginException : Exception
    {
        public FailLoginException() : base("El usuario o contraseña no existen y/o estan incorrectos.")
        {
        }
    }
}
