using AppVidaSana.Api;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.ProducesResponseType.Authenticator;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Exceptions;

namespace AppVidaSana.Controllers
{
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/auth")]
    [EnableRateLimiting("sliding")]
    public class AuthenticatorController : Controller
    {
        private readonly IAccount _AccountService;

        public AuthenticatorController(IAccount AccountService)
        {
            _AccountService = AccountService;
        }

        /// <summary>
        /// This controller performs the login.
        /// </summary>
        /// <response code="200">The start of the session was successful.</response>
        /// <response code="401">Returns a message that you were unable to log in.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>       
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnLoginAccount))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPost("login")]
        [Produces("application/json")]
        public IActionResult LoginAccount([FromBody] LoginAccountDto login)
        {
            try
            {
                TokenUserDto token = _AccountService.LoginAccount(login);

                ReturnLoginAccount response = new ReturnLoginAccount
                {
                    auth = token
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, auth = response.auth });
            }
            catch (LoginException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status401Unauthorized, new { message = response.message, status = response.status });
            }
        }

        /// <summary>
        /// This driver performs password reset.
        /// </summary>
        /// <response code="200">Returns a message indicating that the email has been sent correctly or on the contrary it was not sent because there is no account associated to that email and/or the email could not be sent due to external factors.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPost("forgot-password")]
        [Produces("application/json")]
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

                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    message = true,
                    status = "Se le envió un correo a su bandeja principal."
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, status = response.status });
            }
            catch (EmailNotSendException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, status = response.status });

            }
        }

        /// <summary>
        /// This is the driver for the password reset view.
        /// </summary>
        /// <response code="404">Returns a message indicating that the page could not be loaded correctly.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [AllowAnonymous]
        [HttpGet("ResetPassword")]
        [Produces("application/json")]
        public IActionResult ResetPassword(string token, string email)
        {
            try
            {
                var model = new ResetPasswordDto { token = token, email = email };
                return View(model);
            }
            catch (Exception)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    message = false,
                    status = "No se cargo completamente la página"
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, status = response.status });

            }
        }

        /// <summary>
        /// This controller performs the password reset action.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response> 
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnResetPassword))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPost("reset-password")]
        [Produces("application/json")]
        public IActionResult ResetPassword([FromBody] ResetPasswordDto model)
        {
            try
            {
                var res = _AccountService.ResetPassword(model);
                if (!res)
                {
                    ReturnExceptionMessage statusUpdate = new ReturnExceptionMessage
                    {
                        message = false,
                        status = "Hubo un error al actualizar la contraseña, intentelo de nuevo."
                    };

                    return StatusCode(StatusCodes.Status400BadRequest, new { message = statusUpdate.message, status = statusUpdate.status });
                }

                ReturnResetPassword response = new ReturnResetPassword();

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, status = response.status });
            }
            catch (ComparedPasswordException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });

            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            } 
            catch(ValuesInvalidException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { message = response.message, status = response.status });
            }
        }
    }
}
