namespace AppVidaSana.ProducesReponseType
{
    public class ReturnExceptionMessage
    {
        public string message { get; set; } = "Hubo un error, inténtelo de nuevo.";

        public bool actionStatus { get; set; } = false;

        public string status { get; set; } = null!;

    }
}
