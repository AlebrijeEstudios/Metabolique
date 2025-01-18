namespace AppVidaSana.Exceptions.Account_Profile.ValidationTimeoutException
{
    public class EmailValidationTimeoutException : Exception
    {
        public EmailValidationTimeoutException() : base("La verificación del formato del correo electrónico ha tomado demasiado tiempo.")
        {
        }
    }
}
