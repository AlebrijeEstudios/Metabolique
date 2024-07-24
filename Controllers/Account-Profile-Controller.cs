using AppVidaSana.Api;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.ProducesResponseType.Account;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Exceptions;

namespace AppVidaSana.Controllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/accounts")]
    [EnableRateLimiting("sliding")]
    public class Account_ProfileController : ControllerBase
    {
        private readonly IAccount _AccountService;
        private readonly IProfile _ProfileService;

        public Account_ProfileController(IAccount AccountService, IProfile ProfileService)
        {
            _AccountService = AccountService;
            _ProfileService = ProfileService;
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
        /// <response code="200">Returns account information if found. The information is stored in the attribute called 'response'.</response>
        /// <response code="404">Return an error message if the user is not found. The information is stored in the attribute called 'response'.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnGetAccount))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("{id:guid}")]
        [Produces("application/json")]
        public IActionResult GetAccount(Guid id)
        {
            try
            {
                ReturnAccountDto info = _AccountService.GetAccount(id);

                ReturnGetAccount response = new ReturnGetAccount
                {
                    account = info
                };

                return StatusCode(StatusCodes.Status200OK, new { response });
            }
            catch (UserNotFoundException ex)
            {

                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
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
        /// <response code="201">Returns a token to validate in the app. The information is stored in the attribute called 'response'.</response>
        /// <response code="400">Returns a message that the requested action could not be performed. The information is stored in the attribute called 'response'.</response>
        /// <response code="401">Returns a message that you were unable to log in. The information is stored in the attribute called 'response'.</response>        
        /// <response code="404">Return a message that the user does not exist in the Accounts table. The information is stored in the attribute called 'response'.</response>
        /// <response code="408">Returns a message indicating that the time to scan either the email or password has expired. The information is stored in the attribute called 'timeOut'.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid. The information is stored in the attribute called 'response'.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnCreateAccount))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status408RequestTimeout, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [AllowAnonymous]
        [HttpPost("account-profile")]
        [Produces("application/json")]
        public IActionResult CreateAccount([FromBody] CreateAccountProfileDto account)
        {
            try
            {
                var ac = _AccountService.CreateAccount(account);
                var profile = _ProfileService.CreateProfile(ac.accountID, account);

                if(ac.messageException != "")
                {
                    ReturnExceptionMessage timeOut = new ReturnExceptionMessage
                    {
                        status = ac.messageException
                    };
                    return StatusCode(StatusCodes.Status408RequestTimeout, new { timeOut });
                }

                if (!profile)
                {
                    throw new UnstoredValuesException();
                }

                LoginAccountDto login = new LoginAccountDto
                {
                    email = account.email,
                    password = account.password
                };

                TokenUserDto token = _AccountService.LoginAccount(login);

                ReturnCreateAccount response = new ReturnCreateAccount
                {
                    auth = token
                };

                return StatusCode(StatusCodes.Status201Created, new { response });
            }
            catch (ValuesInvalidException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { response });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { response });
            }
            catch (UserNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
            }
            catch (LoginException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status401Unauthorized, new { response });
            }
            catch (ErrorDatabaseException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { response });
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
        /// <response code="200">Returns a message that the update has been successful. The information is stored in the attribute called 'response'.</response>
        /// <response code="400">Returns a message that the requested action could not be performed. The information is stored in the attribute called 'response'.</response>
        /// <response code="404">Return a message that the user does not exist in the Accounts table. The information is stored in the attribute called 'response'.</response>     
        /// <response code="409">Returns a series of messages indicating that some values are invalid. The information is stored in the attribute called 'response'.</response> 
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnUpdateDeleteAccount))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPut("{id:guid}")] 
        [Produces("application/json")]
        public IActionResult UpdateAccount(Guid id, [FromBody] ReturnAccountDto account)
        {
            try
            {
                var values = _AccountService.UpdateAccount(id, account);
                var res = _ProfileService.UpdateProfile(values.accountID, values);

                ReturnUpdateDeleteAccount response = new ReturnUpdateDeleteAccount
                {
                    status = res
                };

                return StatusCode(StatusCodes.Status200OK, new { response });

            }catch(UserNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { response });
            }
            catch (ValuesInvalidException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { response });
            }
            catch (ErrorDatabaseException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { response });
            }
        }

        /// <summary>
        /// This driver deletes the user's account and everything related to it.
        /// </summary>
        /// <response code="200">Returns a message that the elimination has been successful. The information is stored in the attribute called 'response'.</response>
        /// <response code="400">Returns a message that the requested action could not be performed. The information is stored in the attribute called 'response'.</response>
        /// <response code="404">Return a message that the user does not exist in the Accounts table. The information is stored in the attribute called 'response'.</response> 
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnUpdateDeleteAccount))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpDelete("{id:guid}")]
        [Produces("application/json")]
        public IActionResult DeleteAccount(Guid id)
        {
            try
            {
                var res = _AccountService.DeleteAccount(id);

                ReturnUpdateDeleteAccount response = new ReturnUpdateDeleteAccount
                {
                    status = res
                };

                return StatusCode(StatusCodes.Status200OK, new { response });
            }
            catch (UserNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { response });
            }
        }

    }
}
