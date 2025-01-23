using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Account_Profile.ResetPasswordException;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.Models.Dtos.Reset_Password_Dtos;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.ProducesResponseType.Authenticator;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers
{
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/forgot-password")]
    [RequestTimeout("CustomPolicy")]
    public class ResetPasswordController : Controller
    {
        private readonly IResetPassword _resetPasswordService;

        public ResetPasswordController(IResetPassword resetPasswordService)
        {
            _resetPasswordService = resetPasswordService;
        }

        /// <summary>
        /// This driver performs password reset.
        /// </summary>
        /// <response code="200">Returns a message indicating that the email has been sent correctly or on the contrary it was not sent because there is no account associated to that email and/or the email could not be sent due to external factors.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response> 
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="500">Returns a message indicating internal server errors.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailDto email)
        {
            try
            {
                var token = await _resetPasswordService.PasswordResetTokenAsync(email, HttpContext.RequestAborted);

                var resetLink = Url.Action("ViewResetPassword", "ResetPassword", new { token = token, email = email.email }, Request.Scheme);

                if (resetLink == null) { throw new EmailNotSendException(); }

                _resetPasswordService.SendEmailAsync(email.email, resetLink);

                return StatusCode(StatusCodes.Status200OK);
            }
            catch (EmailNotSendException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, status = response.status });
            }
            catch (ValuesInvalidException ex)
            {
                ExceptionListMessages response = new ExceptionListMessages
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { message = response.message, status = response.status });
            }
            catch (NullTokenException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, new { message = response.message, status = response.status });
            }
        }

        /// <summary>
        /// This is the driver for the password reset view.
        /// </summary>
        /// <response code="404">Returns a message indicating that the page could not be loaded correctly.</response>
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [AllowAnonymous]
        [HttpGet]
        [Produces("application/json")]
        public IActionResult ViewResetPassword(string token, string email)
        {
            try
            {
                var model = new ResetPasswordDto { token = token, email = email };

                return View(model);
            }
            catch (Exception)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    message = "Hubo un error, inténtelo de nuevo.",
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
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="500">Returns a message indicating internal server errors.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResetPasswordResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpPut("reset-password")]
        [Produces("application/json")]
        public async Task<IActionResult> UpdatePassword([FromBody] ResetPasswordDto values)
        {
            try
            {
                var status = await _resetPasswordService.ResetPasswordAsync(values, HttpContext.RequestAborted);

                if (!status)
                {
                    ExceptionMessage error = new ExceptionMessage
                    {
                        message = "Hubo un error, inténtelo de nuevo.",
                        status = "Hubo un error al actualizar la contraseña, intentelo de nuevo."
                    };

                    return StatusCode(StatusCodes.Status400BadRequest, new { message = error.message, status = error.status });
                }

                ResetPasswordResponse response = new ResetPasswordResponse();

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, status = response.status });
            }
            catch (ComparedEmailException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });

            }
            catch (ComparedPasswordException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });

            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (ValuesInvalidException ex)
            {
                ExceptionListMessages response = new ExceptionListMessages
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { message = response.message, status = response.status });
            }
            catch (ErrorDatabaseException ex)
            {
                ExceptionListMessages response = new ExceptionListMessages
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { message = response.message, status = response.status });
            }
            catch (NullTokenException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, new { message = response.message, status = response.status });
            }
        }
    }
}
