namespace AppVidaSana.Exceptions.Account_Profile.ResetPasswordException
{
    public class ComparedPasswordException : Exception
    {
        public ComparedPasswordException() : base("La contrase&ntilde;a no coincide con la confirmada anteriormente.")
        {
        }
    }
}
