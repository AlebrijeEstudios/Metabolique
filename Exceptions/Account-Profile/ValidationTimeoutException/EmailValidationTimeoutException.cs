namespace AppVidaSana.Exceptions.Cuenta_Perfil
{
    public class EmailValidationTimeoutException : Exception
    {
        public EmailValidationTimeoutException() : base("La verificación del formato del correo electrónico ha tomado demasiado tiempo.")
        {
        }
    }
}
