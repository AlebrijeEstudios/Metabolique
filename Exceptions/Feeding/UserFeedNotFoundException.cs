namespace AppVidaSana.Exceptions.Feeding
{
    public class UserFeedNotFoundException : Exception
    {
        public UserFeedNotFoundException() : base("Elemento(s) no encontrado(s).")
        {
        }
    }
}
