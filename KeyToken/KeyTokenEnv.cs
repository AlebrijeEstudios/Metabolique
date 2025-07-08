using AppVidaSana.Exceptions;

namespace AppVidaSana.KeyToken
{
    public static class KeyTokenEnv
    {
        public static string GetKeyTokenEnv()
        {
            return Environment.GetEnvironmentVariable("TOKEN") ??
                   Environment.GetEnvironmentVariable("TOKEN_Replacement") ??
                   throw new NullTokenException();
        }

        public static string GetTokenIssuerEnv()
        {
            return Environment.GetEnvironmentVariable("TOKEN_ISSUER");
        }

        public static string GetTokenAudienceEnv()
        {
            return Environment.GetEnvironmentVariable("TOKEN_AUDIENCE");
        }
    }
}
