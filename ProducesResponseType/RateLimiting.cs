namespace AppVidaSana.ProducesResponseType
{
    public class RateLimiting
    {
        public string message { get; set; } = "Exceso de llamadas al servidor, por favor prueba mas tarde.";

        public string status { get; set; } = "Alcanzo el limite de peticiones permitidas, espere un lapso de 12 segundos para continuar";
    }
}
