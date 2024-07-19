using AppVidaSana.Api;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Services.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;

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
        private string mensaje = "Hubo un error, intentelo de nuevo.";

        public AccountController(IAccount AccountService, IProfile ProfileService, IMapper mapper)
        {
            _AccountService = AccountService;
            _ProfileService = ProfileService;
            _mapper = mapper;
        }

        [ApiKeyAuthorizationFilter]
        [HttpGet("{id:guid}")]
        public IActionResult GetAccount(Guid id)
        {
            try
            {
                ReturnAccountDto infoAccount = _AccountService.GetAccount(id);
                return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = infoAccount });
            }
            catch (UserNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = mensaje, response = ex.Message });
            }
        }

        [ApiKeyAuthorizationFilter]
        [AllowAnonymous]
        [HttpPost("account-profile")]
        public IActionResult CreateAccount([FromBody] CreateAccountProfileDto account)
        {
            try
            {
                var ac = _AccountService.CreateAccount(account);
                var profile = _ProfileService.CreateProfile(ac.accountID, account);

                if (!profile)
                {
                    throw new ValuesVoidException();
                }

                LoginAccountDto login = new LoginAccountDto
                {
                    email = account.email,
                    password = account.password
                };

                TokenUserDto token = _AccountService.LoginAccount(login);
               
                return StatusCode(StatusCodes.Status201Created, new { message = "ok", response = token });
            }
            catch (ValuesInvalidException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = ex.Errors });
            }
            catch (ValuesVoidException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = ex.Message });
            }
            catch (UserNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = ex.Message });
            }
            catch (LoginException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = ex.Message });
            }
            catch (ErrorDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = ex.Errors });
            }
        }

        [ApiKeyAuthorizationFilter]
        [HttpPut("{id:guid}")]
        public IActionResult UpdateAccount(Guid id, [FromBody] CreateAccountProfileDto account)
        {
            try
            {
                var values = _AccountService.UpdateAccount(id, account);
                var res = _ProfileService.UpdateProfile(values.accountID, values);

                return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = res });

            }catch(UserNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = mensaje, response = ex.Message });
            }
            catch (ValuesVoidException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = ex.Message });
            }
            catch (ValuesInvalidException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = ex.Errors });
            }
            catch (ErrorDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = ex.Errors });
            }
        }

        [ApiKeyAuthorizationFilter]
        [HttpDelete("{id:guid}")]
        public IActionResult DeleteAccount(Guid id)
        {
            try
            {
                var res = _AccountService.DeleteAccount(id);
                return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = res });
            }
            catch (UserNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = mensaje, response = ex.Message });
            }
            catch (ValuesVoidException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = ex.Message });
            }
        }

    }
}
