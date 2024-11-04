namespace AppVidaSana.Exceptions.Cuenta_Perfil
{
    public class EmailNotSendException : Exception
    {
        public EmailNotSendException() : base("No se logró enviar el correo, inténtelo de nuevo.")
        {
        }
    }
}
