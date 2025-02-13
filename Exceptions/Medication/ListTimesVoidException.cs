namespace AppVidaSana.Exceptions.Medication
{
    public class ListTimesVoidException : Exception
    {
        public ListTimesVoidException() : base("Debe agregar al menos un horario, int&eacute;ntelo de nuevo.") { }

    }
}
