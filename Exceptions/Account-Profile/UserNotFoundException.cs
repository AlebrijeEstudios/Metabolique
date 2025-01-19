namespace AppVidaSana.Exceptions.Account_Profile
{
    public class UserNotFoundException : Exception
    {
        public UserNotFoundException() : base("Usuario no encontrado.")
        {
        }
    }
}
