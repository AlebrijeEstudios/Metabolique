namespace AppVidaSana.Exceptions.Medication
{
    public class ErrorRangeDatesException : Exception
    {
        public ErrorRangeDatesException() : base("La fecha final debe ser despu&eacute;s de la fecha inicial, int&eacute;ntelo de nuevo.") { }

    }
}
