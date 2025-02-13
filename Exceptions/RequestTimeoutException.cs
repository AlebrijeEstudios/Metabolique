namespace AppVidaSana.Exceptions
{
    public class RequestTimeoutException : Exception
    {
        public RequestTimeoutException() : base("La petici&oacute;n ha tardado m&aacute;s de lo esperado, int&eacute;ntelo de nuevo.")
        {
        }
    }
}
