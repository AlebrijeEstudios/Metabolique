namespace AppVidaSana.ProducesResponseType
{
    public class ExpiredTokenException
    {
        public int status { get; set; }
        public string error { get; set; } = null!; 
        public string message { get; set; } = null!;
        public string timestamp { get; set; } = null!;
        public string path { get; set; } = null!;
    }
}
