namespace AppVidaSana.Exceptions.Account_Profile
{
    public class RefreshTokenExpirationException : Exception
    {
        public RefreshTokenExpirationException() : base("El refresh token expiró, inicie sesión.")
        {
        }
    }
}
