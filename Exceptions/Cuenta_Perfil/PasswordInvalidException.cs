namespace AppVidaSana.Exceptions.Cuenta_Perfil
{
    public class PasswordInvalidException : Exception
    {
        public PasswordInvalidException() : base("La contraseña debe contener letras mayúsculas o minúsculas, al menos un número y un carácter alfanumérico.")
        {
        }
    }
}
