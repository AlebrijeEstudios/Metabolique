namespace AppVidaSana.Exceptions.Account_Profile
{
    public class NoRoleAssignmentException : Exception
    {
        public NoRoleAssignmentException() : base("Hubo un error al asignar el rol del usuario.")
        {
        }
    }
}
