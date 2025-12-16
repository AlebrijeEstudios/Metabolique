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
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.ProducesResponseType.ResponseOperationsFilters.ApiResponsesAttribute;

namespace AppVidaSana.Controllers
{
    [AllowAnonymous]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("App - Forgot_Password")]
    [ApiExplorerSettings(GroupName = "app")]
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
        [CommonApiResponse]
        [BadRequestApiResponse]
        [ConflictApiResponse]
        [InternalServerErrorApiResponse]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("forgot-password")]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> ForgotPassword([FromBody] EmailDto email)
        {
            try
            {
                var token = await _resetPasswordService.PasswordResetTokenAsync(email, HttpContext.RequestAborted);

                var resetLink = Url.Action("ViewResetPassword", "ResetPassword", new { token = token }, Request.Scheme);

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
        [HttpGet]
        public IActionResult ViewResetPassword(string token)
        {
            try
            {
                var model = new ResetPasswordDto { token = token };

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
        [CommonApiResponse]
        [BadRequestApiResponse]
        [UnauthorizedApiResponse]
        [ConflictApiResponse]
        [InternalServerErrorApiResponse]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResetPasswordResponse))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("reset-password")]
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