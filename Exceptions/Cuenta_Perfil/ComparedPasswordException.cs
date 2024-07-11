namespace AppVidaSana.Exceptions.Cuenta_Perfil
{
    public class ComparedPasswordException : Exception
    {
        public ComparedPasswordException() : base("La contraseña no coincide con la confirmada anteriormente.")
        {
        }
    }
}
