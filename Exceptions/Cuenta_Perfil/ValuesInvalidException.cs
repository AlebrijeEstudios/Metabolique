namespace AppVidaSana.Exceptions.Cuenta_Perfil
{
    public class ValuesInvalidException : Exception
    {
        public List<string?> Errors { get; }

        public ValuesInvalidException(List<string?> errors) 
        {
            Errors = errors;
        }
    }
}
