namespace AppVidaSana.Exceptions.Account_Profile
{
    public class ValuesInvalidException : Exception
    {
        public List<string?> Errors { get; }

        public ValuesInvalidException(List<string?> errors)
        {
            Errors = errors;
        }
    }
}
