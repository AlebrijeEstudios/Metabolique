namespace AppVidaSana.Exceptions
{
    public class RepeatRegistrationException : Exception
    {
        public RepeatRegistrationException() : base("Esta informaci&oacute;n ya fue registrada, int&eacute;ntelo con nuevos valores.")
        {
        }
    }
}
