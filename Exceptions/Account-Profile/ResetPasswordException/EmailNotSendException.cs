namespace AppVidaSana.Exceptions.Account_Profile.ResetPasswordException
{
    public class EmailNotSendException : Exception
    {
        public EmailNotSendException() : base("No se logr&oacute; enviar el correo, int&eacute;ntelo de nuevo.")
        {
        }
    }
}
