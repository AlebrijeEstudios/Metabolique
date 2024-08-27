namespace AppVidaSana.ProducesReponseType
{
    public class ReturnExceptionList
    {
        public string message { get; set; } = "Hubo un error, inténtelo de nuevo.";

        public List<string?> status { get; set; } = null!;

    }
}
