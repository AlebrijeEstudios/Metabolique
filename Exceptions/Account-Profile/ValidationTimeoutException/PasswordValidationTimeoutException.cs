namespace AppVidaSana.Exceptions.Account_Profile.ValidationTimeoutException
{
    public class PasswordValidationTimeoutException : Exception
    {
        public PasswordValidationTimeoutException() : base("La verificaci&oacute;n del formato de la contrase&ntilde;a ha tomado demasiado tiempo.")
        {
        }
    }
}
