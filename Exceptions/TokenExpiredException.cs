namespace AppVidaSana.Exceptions
{
    public class TokenExpiredException : Exception
    {
        public TokenExpiredException() : base("El token ha caducado, vuelva a autenticarse.")
        {
        }
    }
}
