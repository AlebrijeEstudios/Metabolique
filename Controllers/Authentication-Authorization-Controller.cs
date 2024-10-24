using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.Reset_Password_Dtos;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType.Authenticator;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers
{
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/auth")]
    [RequestTimeout("CustomPolicy")]
    public class AuthenticationAuthorizationController : ControllerBase
    {
        private readonly IAuthentication_Authorization _AuthService;

        public AuthenticationAuthorizationController(IAuthentication_Authorization AuthService)
        {
            _AuthService = AuthService;
        }

        /// <summary>
        /// This controller performs the login.
        /// </summary>
        /// <response code="200">The start of the session was successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message that you were unable to log in.</response>  
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseLogin))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ApiKeyAuthorizationFilter]
        [HttpPost("login")]
        [Produces("application/json")]
        public async Task<IActionResult> LoginAccount([FromBody] LoginDto login)
        {
            try
            {
                var token = await _AuthService.LoginAccount(login, HttpContext.RequestAborted);

                ResponseLogin response = new ResponseLogin
                {
                    auth = token
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, auth = response.auth });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (FailLoginException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status401Unauthorized, new { message = response.message, status = response.status });
            }
            catch (ErrorDatabaseException ex)
            {
                ExceptionListMessages response = new ExceptionListMessages
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { message = response.message, status = response.status });
            }
        }

        /// <summary>
        /// This controller generates new tokens.
        /// </summary>
        /// <response code="200">The tokens were successfully generated.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseLogin))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ApiKeyAuthorizationFilter]
        [HttpPost("refresh-token")]
        [Produces("application/json")]
        public async Task<IActionResult> RefreshToken([FromBody] TokensDto values)
        {
            try
            {
                var tokens = await _AuthService.RefreshToken(values, HttpContext.RequestAborted);

                ResponseLogin response = new ResponseLogin
                {
                    auth = tokens
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, auth = response.auth });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (ErrorDatabaseException ex)
            {
                ExceptionListMessages response = new ExceptionListMessages
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { message = response.message, status = response.status });
            }
        }

    }
}
