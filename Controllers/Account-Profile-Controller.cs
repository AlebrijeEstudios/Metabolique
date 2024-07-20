using AppVidaSana.Api;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Services.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.ProducesResponseType.Account;

namespace AppVidaSana.Controllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccount _AccountService;
        private readonly IProfile _ProfileService;
        private readonly IMapper _mapper;

        public AccountController(IAccount AccountService, IProfile ProfileService, IMapper mapper)
        {
            _AccountService = AccountService;
            _ProfileService = ProfileService;
            _mapper = mapper;
        }

        /// <summary>
        /// This controller obtains the user's account and profile.
        /// </summary>
        /// <response code="200">Returns account information if found.</response>
        /// <response code="404">Return an error message if the user is not found.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnGetAccount))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("{id:guid}")]
        [Produces("application/json")]
        public IActionResult GetAccount(Guid id)
        {
            try
            {
                ReturnAccountDto info = _AccountService.GetAccount(id);

                ReturnGetAccount infoAccount = new ReturnGetAccount
                {
                    response = info
                };

                return StatusCode(StatusCodes.Status200OK, new { infoAccount });
            }
            catch (UserNotFoundException ex)
            {

                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    response = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
            }
        }

        /// <summary>
        /// This controller creates the user's account.
        /// </summary>
        /// <response code="201">Returns a token to validate in the app.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Return a message that the user does not exist in the Accounts table.</response>
        /// <response code="401">Returns a message that you were unable to log in.</response>        
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnCreateAccount))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
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
                    response = token
                };

                return StatusCode(StatusCodes.Status201Created, new { response });
            }
            catch (ValuesInvalidException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    response = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { response });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    response = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { response });
            }
            catch (UserNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    response = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
            }
            catch (LoginException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    response = ex.Message
                };

                return StatusCode(StatusCodes.Status401Unauthorized, new { response });
            }
            catch (ErrorDatabaseException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    response = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { response });
            }
        }

        /// <summary>
        /// This driver updates the user's account.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Return a message that the user does not exist in the Accounts table.</response>     
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Return_Update_Delete_Account))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ApiKeyAuthorizationFilter]
        [HttpPut("{id:guid}")] 
        [Produces("application/json")]
        public IActionResult UpdateAccount(Guid id, [FromBody] CreateAccountProfileDto account)
        {
            try
            {
                var values = _AccountService.UpdateAccount(id, account);
                var res = _ProfileService.UpdateProfile(values.accountID, values);

                Return_Update_Delete_Account response = new Return_Update_Delete_Account
                {
                    response = res
                };

                return StatusCode(StatusCodes.Status200OK, new { response });

            }catch(UserNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    response = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    response = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { response });
            }
            catch (ValuesInvalidException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    response = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { response });
            }
            catch (ErrorDatabaseException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    response = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { response });
            }
        }

        /// <summary>
        /// This driver deletes the user's account and everything related to it.
        /// </summary>
        /// <response code="200">Returns a message that the elimination has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Return a message that the user does not exist in the Accounts table.</response>     
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Return_Update_Delete_Account))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpDelete("{id:guid}")]
        [Produces("application/json")]
        public IActionResult DeleteAccount(Guid id)
        {
            try
            {
                var res = _AccountService.DeleteAccount(id);

                Return_Update_Delete_Account response = new Return_Update_Delete_Account
                {
                    response = res
                };

                return StatusCode(StatusCodes.Status200OK, new { response });
            }
            catch (UserNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    response = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    response = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { response });
            }
        }

    }
}
