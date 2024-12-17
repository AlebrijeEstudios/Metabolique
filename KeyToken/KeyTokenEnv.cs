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
    }
}
