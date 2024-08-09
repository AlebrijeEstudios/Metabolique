namespace AppVidaSana.ProducesResponseType
{
    public class RateLimiting
    {
        public bool message { get; set; } = false;

        public string status { get; set; } = "Alcanzo el limite de peticiones permitidas, espere un lapso de 10 segundos para continuar.";
    }
}
