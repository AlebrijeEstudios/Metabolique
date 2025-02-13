namespace AppVidaSana.Exceptions
{
    public class NullTokenException : Exception
    {
        public NullTokenException() : base("El keyToken no est&aacute; configurado.")
        {
        }
    }
}
