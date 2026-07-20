namespace AppVidaSana.Exceptions
{
    public class SelfActionNotAllowedException : Exception
    {
        public SelfActionNotAllowedException(string message) : base(message)
        {
        }
    }
}
