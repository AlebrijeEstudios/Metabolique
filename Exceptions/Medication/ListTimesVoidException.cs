namespace AppVidaSana.Exceptions.Medication
{
    public class ListTimesVoidException : Exception
    {
        public ListTimesVoidException() : base("Debe agregar al menos un horario, inténtelo de nuevo.") { }

    }
}
