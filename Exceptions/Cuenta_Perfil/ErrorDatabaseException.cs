namespace AppVidaSana.Exceptions.Cuenta_Perfil
{
    public class ErrorDatabaseException : Exception
    {
        public List<string> Errors { get; }

        public ErrorDatabaseException(List<string> errors)
        {
            Errors = errors;
        }
    }
}
