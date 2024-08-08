namespace AppVidaSana.ProducesReponseType
{
    public class ReturnExceptionList
    {
        public string message { get; set; } = "Hubo un error, inténtelo de nuevo.";

        public bool actionStatus { get; set; } = false;

        public List<string?> status { get; set; } = null!;

    }
}
