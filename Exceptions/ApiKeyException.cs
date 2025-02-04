namespace AppVidaSana.Exceptions
{
    public class ApiKeyException : Exception
    {
        public ApiKeyException() : base("La api key no coincide con el colocado por usted.")
        {
        }
    }
}
