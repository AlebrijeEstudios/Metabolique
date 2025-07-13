using AppVidaSana.Api;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.Exceptions;
using AppVidaSana.ProducesResponseType;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;
using AppVidaSana.ProducesResponseType.AdminWeb;
using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.AspNetCore.RateLimiting;

namespace AppVidaSana.Controllers.AdminWeb
{
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Admin - Auth")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Route("api/admin/auth")]
    [RequestTimeout("CustomPolicy")]
    public class AdminAuthController : ControllerBase
    {
        private readonly IAWAuth _AuthService;

        public AdminAuthController(IAWAuth AuthService)
        {
            _AuthService = AuthService;
        }

        /// <summary>
        /// This controller performs the login admin web.
        /// </summary>
        /// <response code="200">The start of the session was successful.</response>
        /// <response code="401">Returns a message that you were unable to log in.</response>  
        /// <response code="500">Returns a message indicating internal server errors.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAuthResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("login")]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> LoginAdminAsync([FromBody] LoginAdminDto login)
        {
            try
            {
                var token = await _AuthService.LoginAdminAsync(login, HttpContext.RequestAborted);

                GetAuthResponse response = new GetAuthResponse
                {
                    auth = token
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, auth = response.auth });
            }
            catch (FailLoginException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status401Unauthorized, new { message = response.message, status = response.status });
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
