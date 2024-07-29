namespace AppVidaSana.Exceptions
{
    public class RepeatRegistrationException : Exception
    {
        public RepeatRegistrationException() : base("Esta información ya fue registrada, inténtelo con nuevos valores.")
        {
        }
    }
}
