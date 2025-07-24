using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.ProducesResponseType.Account_Profile;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.Models.Dtos.Reset_Password_Dtos;
using AppVidaSana.ProducesResponseType.Authenticator;

namespace AppVidaSana.TESTS
{
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("App - TESTS")]
    [ApiExplorerSettings(GroupName = "app_test")]
    [Route("test")]
    [RequestTimeout("CustomPolicy")]
    public class ServicesTestsController : ControllerBase
    {
        private readonly IServicesTests _testServices;
        private readonly IProfile _ProfileService;

        public ServicesTestsController(IServicesTests testServices,IProfile ProfileService)
        {
            _testServices = testServices;
            _ProfileService = ProfileService;
        }

        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AuthResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [AllowAnonymous]
        [EnableRateLimiting("write")]
        [HttpPost("account-profile")]
        [Produces("application/json")]
        [RequestTimeout("CustomPolicy")]
        public async Task<IActionResult> CreateAccountAsync([FromBody] AccountDto values)
        {
            try
            {
                var accountID = await _testServices.CreateAccountAsync(values, HttpContext.RequestAborted);

                await _ProfileService.CreateProfileAsync(accountID, values, HttpContext.RequestAborted);

                LoginDto login = new LoginDto
                {
                    email = values.email,
                    password = values.password
                };

                var token = await _testServices.LoginAccountAsync(login, HttpContext.RequestAborted);

                AuthResponse response = new AuthResponse
                {
                    auth = token
                };

                return StatusCode(StatusCodes.Status201Created, new { message = response.message, auth = response.auth });
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
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("login")]
        [HttpPost("login")]
        [Produces("application/json")]
        public async Task<IActionResult> LoginAccountAsync([FromBody] LoginDto login)
        {
            try
            {
                var token = await _testServices.LoginAccountAsync(login, HttpContext.RequestAborted);

                LoginResponse response = new LoginResponse
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
            catch (NullTokenException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status500InternalServerError, new { message = response.message, status = response.status });
            }
        }

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LoginResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("refresh")]
        [HttpPost("refresh-token")]
        [Produces("application/json")]
        public async Task<IActionResult> RefreshTokenAsync([FromBody] TokensDto values)
        {
            try
            {
                var tokens = await _testServices.RefreshTokenAsync(values, HttpContext.RequestAborted);

                LoginResponse response = new LoginResponse
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
            catch (RefreshTokenExpirationException ex)
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

        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(LogoutResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("write")]
        [HttpDelete("logout/{accountID:guid}")]
        [Produces("application/json")]
        public async Task<IActionResult> LogoutAccountAsync(Guid accountID)
        {
            try
            {
                var status = await _testServices.LogoutAccountAsync(accountID, HttpContext.RequestAborted);

                LogoutResponse response = new LogoutResponse
                {
                    status = status
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, status = response.status });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
        }
    }
}
