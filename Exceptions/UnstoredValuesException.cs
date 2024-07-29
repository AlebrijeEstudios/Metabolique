namespace AppVidaSana.Exceptions
{
    public class UnstoredValuesException : Exception
    {
        public UnstoredValuesException() : base("No se puede realizar la acción requerida, inténtelo de nuevo.")
        {
        }
    }
}
