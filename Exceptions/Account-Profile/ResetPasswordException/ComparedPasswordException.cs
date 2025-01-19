namespace AppVidaSana.Exceptions.Account_Profile.ResetPasswordException
{
    public class ComparedPasswordException : Exception
    {
        public ComparedPasswordException() : base("La contraseña no coincide con la confirmada anteriormente.")
        {
        }
    }
}
