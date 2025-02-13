namespace AppVidaSana.Exceptions.Account_Profile
{
    public class RefreshTokenExpirationException : Exception
    {
        public RefreshTokenExpirationException() : base("El refresh token expir&oacute;, inicie sesi&oacute;n.")
        {
        }
    }
}
