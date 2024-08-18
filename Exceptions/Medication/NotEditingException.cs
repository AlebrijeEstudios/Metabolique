namespace AppVidaSana.Exceptions.Medication
{
    public class NotEditingException : Exception
    {
        public NotEditingException() : base("No puede editar periodos pasados.") { }

    }
}
