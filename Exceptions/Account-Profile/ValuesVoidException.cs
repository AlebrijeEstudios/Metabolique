namespace AppVidaSana.Exceptions.Account_Profile
{
    public class ValuesVoidException : Exception
    {
        public ValuesVoidException() : base("No se guardaron los datos, intentelo de nuevo.")
        {
        }
    }
}
