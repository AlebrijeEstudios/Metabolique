namespace AppVidaSana.Exceptions
{
    public class RequestTimeoutException : Exception
    {
        public RequestTimeoutException() : base("La petición ha tardado más de lo esperado, intentelo de nuevo.")
        {
        }
    }
}
