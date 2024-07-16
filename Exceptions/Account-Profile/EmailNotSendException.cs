namespace AppVidaSana.Exceptions.Cuenta_Perfil
{
    public class EmailNotSendException : Exception
    {
        public EmailNotSendException() : base("No se logro enviar el correo, intentelo de nuevo")
        {
        }
    }
}
