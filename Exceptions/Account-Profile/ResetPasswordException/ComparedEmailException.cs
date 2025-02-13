namespace AppVidaSana.Exceptions.Account_Profile.ResetPasswordException
{
    public class ComparedEmailException : Exception
    {
        public ComparedEmailException() : base("Autenticaci&oacute;n fallida: El correo no corresponde al del usuario autenticado.")
        {
        }
    }
}
