using System.Text.RegularExpressions;

namespace AppVidaSana.RegexPatterns
{
    public static class RegexPatterns
    {
        public static Regex Emailregex { get; } = new Regex(@"[\w!#$%&'*+/=?^_`{|}~-]+(?:\.[a-zA-Z0-9_-]+)*@(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]*[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}",
            RegexOptions.None, TimeSpan.FromMilliseconds(100));


        public static Regex Passwordregex { get; } = new Regex(@"(?=.*[a-zA-Z])(?=.*\d)(?=.*[!""#$%&'()*+,-./:;=?@[\]^_`{|}~])[\w!""#$%&'()*+,-./:;=?@[\]^_`{|}~]",
            RegexOptions.None, TimeSpan.FromMilliseconds(100));

    }
}
