using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticatorController : Controller
    {
        private readonly IAccount _AccountService;
        private string mensaje = "Hubo un error, intentelo de nuevo.";

        public AuthenticatorController(IAccount AccountService)
        {
            _AccountService = AccountService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult LoginAccount([FromBody] LoginAccountDto login)
        {
            try
            {
                TokenUserDto token = _AccountService.LoginAccount(login);
                return StatusCode(StatusCodes.Status202Accepted, new { message = "ok", response = token });
            }
            catch (LoginException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = mensaje, response = ex.Message });
            }
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDto req)
        {
            try
            {
                var tk = _AccountService.RequestPasswordResetToken(req);
                var resetLink = Url.Action("ResetPassword", "Authenticator", new { token = tk.token, email = req.email }, Request.Scheme);

                if (resetLink != null)
                {
                    _AccountService.SendPasswordResetEmail(req.email, resetLink);
                }
                else
                {
                    throw new EmailNotSendException();
                }

                return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = "Se le envio un correo a su bandeja principal" });
            }
            catch (EmailNotSendException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = ex.Message });

            }
        }

        [AllowAnonymous]
        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword(string token, string email)
        {
            try
            {
                var model = new ResetPasswordDto { token = token, email = email };
                return View(model);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = "No se cargo completamente" });

            }

        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDto model)
        {
            try
            {
                var res = _AccountService.ResetPassword(model);
                if (!res)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { message = "Error", response = "Hubo un error, intentelo de nuevo" });
                }

                return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = "La contraseña se actualizo correctamente" });
            }
            catch (ComparedPasswordException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = ex.Message});

            }
            catch(ValuesInvalidException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje , response = ex.Errors });
            }
        }

    }
}
