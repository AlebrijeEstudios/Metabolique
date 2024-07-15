using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AppVidaSana.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticatorController : Controller
    {
        private readonly ICuenta _uRepo;

        public AuthenticatorController(ICuenta uRepo)
        {
            _uRepo = uRepo;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public IActionResult LoginUser([FromBody] LoginUserDto login)
        {
            try
            {
                TokenUserDto tk = _uRepo.LoginUser(login);
                return StatusCode(StatusCodes.Status202Accepted, new { mensaje = "ok", response = tk });
            }
            catch (LoginException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Hubo un error, intentelo de nuevo", response = ex.Message });

            }
        }

        [HttpPost("forgot-password")]
        public IActionResult ForgotPassword([FromBody] ForgotPasswordDto req)
        {
            try
            {
                var token = _uRepo.RequestPasswordResetToken(req);
                var resetLink = Url.Action("ResetPassword", "Authenticator", new { token = token.Token, email = req.email }, Request.Scheme);

                if (resetLink != null)
                {
                    _uRepo.SendPasswordResetEmail(req.email, resetLink);
                }

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = "Se le envio un correo a su bandeja principal" });
            }
            catch (EmailNotSendException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Hubo un error, intentelo de nuevo", response = ex.Message });

            }
        }

        [HttpGet("ResetPassword")]
        public IActionResult ResetPassword(string token, string email)
        {
            try
            {
                var model = new ResetPasswordDto { token = token, email = email };
                return View(model);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = ex.Message, response = "No se cargo completamente" });

            }

        }

        [HttpPost("reset-password")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDto model)
        {
            try
            {
                try
                { 
                    var result = _uRepo.ResetPassword(model);
                    if (!result)
                    {
                        return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", response = "Hubo un error, intentelo de nuevo" });
                    }

                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "Ok", response = "La contraseña se actualizo correctamente" });
                }
                catch (ComparedPasswordException ex)
                {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", response = ex.Message});

                }
            }catch(ValuesInvalidException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", response = ex.Errors });
            }

            
        }

    }
}
