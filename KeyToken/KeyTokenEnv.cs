using AppVidaSana.Exceptions;

namespace AppVidaSana.KeyToken
{
    public class KeyTokenEnv
    {
        public string GetKeyTokenEnv()
        {
            return Environment.GetEnvironmentVariable("TOKEN") ??
                   Environment.GetEnvironmentVariable("TOKEN_Replacement") ??
                   throw new NullTokenException();
        }
    }
}
