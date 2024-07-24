namespace AppVidaSana.Exceptions
{
    public class ErrorDatabaseException : Exception
    {
        public List<string?> Errors { get; }

        public ErrorDatabaseException(List<string?> errors)
        {
            Errors = errors;
        }
    }
}
