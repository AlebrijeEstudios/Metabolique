namespace AppVidaSana.Exceptions.Account_Profile.ResetPasswordException
{
    public class EmailNotSendException : Exception
    {
        public EmailNotSendException() : base("No se logró enviar el correo, inténtelo de nuevo.")
        {
        }
    }
}
