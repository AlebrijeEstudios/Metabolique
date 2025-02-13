namespace AppVidaSana.Exceptions.Account_Profile
{
    public class FailLoginException : Exception
    {
        public FailLoginException() : base("El usuario o contrase&ntilde;a no existen y/o estan incorrectos.")
        {
        }
    }
}
