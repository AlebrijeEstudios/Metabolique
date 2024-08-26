namespace AppVidaSana.ProducesResponseType.Authenticator
{
    public class ReturnResetPassword
    {
        public bool message { get; set; } = true;

        public string status { get; set; } = "La contraseña se actualizo correctamente";
    }
}
