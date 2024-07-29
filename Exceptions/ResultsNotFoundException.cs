namespace AppVidaSana.Exceptions
{
    public class ResultsNotFoundException : Exception
    {
        public ResultsNotFoundException() : base("No se tiene registrado los resultados de la encuesta.")
        {
        }
    }
}
