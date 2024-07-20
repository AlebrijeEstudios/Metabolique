namespace AppVidaSana.Exceptions.Account_Profile
{
    public class UnstoredValuesException : Exception
    {
        public UnstoredValuesException() : base("No se puedo realizar la acción requerida, intentelo de nuevo.")
        {
        }
    }
}
