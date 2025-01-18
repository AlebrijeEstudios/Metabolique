namespace AppVidaSana.Exceptions.Account_Profile.ValidationTimeoutException
{
    public class PasswordValidationTimeoutException : Exception
    {
        public PasswordValidationTimeoutException() : base("La verificación del formato de la contraseña ha tomado demasiado tiempo.")
        {
        }
    }
}
