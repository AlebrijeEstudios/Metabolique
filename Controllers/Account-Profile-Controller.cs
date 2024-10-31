using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.ProducesResponseType.Account;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/accounts")]
    [RequestTimeout("CustomPolicy")]
    public class AccountProfileController : ControllerBase
    {
        private readonly IAccount _AccountService;
        private readonly IProfile _ProfileService;
        private readonly IAuthentication_Authorization _AuthService;

        public AccountProfileController(IAccount AccountService, IProfile ProfileService, IAuthentication_Authorization AuthService)
        {
            _AccountService = AccountService;
            _ProfileService = ProfileService;
            _AuthService = AuthService;
        }

        /// <summary>
        /// This controller obtains the user's account and profile.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The birthDate property must have the following structure:   
        ///     {
        ///        "birthDate": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns account information if found.</response>
        /// <response code="404">Return an error message if the user is not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseAccount))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("{accountID:guid}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetAccount(Guid accountID)
        {
            try
            {
                var account = await _AccountService.GetAccountAsync(accountID, HttpContext.RequestAborted);

                ResponseAccount response = new ResponseAccount
                {
                    account = account
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, account = response.account });
            }
            catch (UserNotFoundException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, status = response.status });
            }
        }

        /// <summary>
        /// This controller creates the user's account.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The birthDate property must have the following structure:   
        ///     {
        ///        "birthDate": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///     
        /// </remarks>
        /// <response code="201">Returns a token to validate in the app.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message that you were unable to log in.</response>        
        /// <response code="404">Return a message that the user does not exist in the Accounts table.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseAuth))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ApiKeyAuthorizationFilter]
        [AllowAnonymous]
        [HttpPost("account-profile")]
        [Produces("application/json")]
        [RequestTimeout("CustomPolicy")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountDto values)
        {
            try
            {
                var accountID = await _AccountService.CreateAccountAsync(values, HttpContext.RequestAborted);

                _ProfileService.CreateProfileAsync(accountID, values, HttpContext.RequestAborted);

                LoginDto login = new LoginDto
                {
                    email = values.email,
                    password = values.password
                };

                var token = await _AuthService.LoginAccountAsync(login, HttpContext.RequestAborted);

                ResponseAuth response = new ResponseAuth
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
            catch (NoRoleAssignmentException ex)
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

        /// <summary>
        /// This driver updates the user's account.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The birthDate property must have the following structure:   
        ///     {
        ///        "birthDate": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns a message that the update has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Return a message that the user does not exist in the Accounts table.</response>     
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response> 
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ApiKeyAuthorizationFilter]
        [HttpPut]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateAccount([FromBody] InfoAccountDto values)
        {
            try
            {
                var profile = await _AccountService.UpdateAccountAsync(values, HttpContext.RequestAborted);
                var message = await _ProfileService.UpdateProfileAsync(profile, HttpContext.RequestAborted);

                ResponseMessage response = new ResponseMessage
                {
                    status = message
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
            catch (UserNotFoundException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, status = response.status });
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

        /// <summary>
        /// This driver deletes the user's account and everything related to it.
        /// </summary>
        /// <response code="200">Returns a message that the elimination has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Return a message that the user does not exist in the Accounts table.</response> 
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpDelete("{accountID:guid}")]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteAccount(Guid accountID)
        {
            try
            {
                var message = await _AccountService.DeleteAccountAsync(accountID, HttpContext.RequestAborted);

                ResponseMessage response = new ResponseMessage
                {
                    status = message
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
            catch (UserNotFoundException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, status = response.status });
            }
        }

    }
}
