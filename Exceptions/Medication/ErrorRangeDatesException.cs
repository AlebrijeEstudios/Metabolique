namespace AppVidaSana.Exceptions.Medication
{
    public class ErrorRangeDatesException : Exception
    {
        public ErrorRangeDatesException() : base("La fecha final debe ser después de la fecha inicial, inténtelo de nuevo.") { }

    }
}
