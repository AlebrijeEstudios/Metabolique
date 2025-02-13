namespace AppVidaSana.Exceptions.Account_Profile.ValidationTimeoutException
{
    public class EmailValidationTimeoutException : Exception
    {
        public EmailValidationTimeoutException() : base("La verificaci&oacute;n del formato del correo electr&oacute;nico ha tomado demasiado tiempo.")
        {
        }
    }
}
