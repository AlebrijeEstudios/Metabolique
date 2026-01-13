namespace AppVidaSana.Exceptions
{
    public class NullTokenException : Exception
    {
        public NullTokenException() : base("El api key no est&aacute; configurado.")
        {
        }
    }
}
