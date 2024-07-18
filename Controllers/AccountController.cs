using AppVidaSana.Api;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Services.IServices;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly IAccount _AccountService;
        private readonly IMapper _mapper;
        private string mensaje = "Hubo un error, intentelo de nuevo.";

        public AccountController(IAccount AccountService, IMapper mapper)
        {
            _AccountService = AccountService;
            _mapper = mapper;
        }

        [ApiKeyAuthorizationFilter]
        [HttpGet("{id:guid}")]
        public IActionResult GetAccount(Guid id)
        {
            try
            {
                AccountInfoDto infoAccount = _AccountService.GetAccount(id);
                return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = infoAccount });
            }
            catch (UserNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = mensaje, response = ex.Message });
            }
        }

        [ApiKeyAuthorizationFilter]
        [HttpPost]
        public IActionResult CreateAccount([FromBody] RegisterUserDto account)
        {
            try
            {
                var res = _AccountService.CreateAccount(account);
                return StatusCode(StatusCodes.Status201Created, new { message = "ok", response = res });
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
        [HttpPut("{id:guid}")]
        public IActionResult UpdateAccount(Guid id, [FromBody] AccountInfoDto infoAccount)
        {
            try
            {
                var res = _AccountService.UpdateAccount(id, infoAccount);
                return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = res });

            }catch(UserNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = mensaje, response = ex.Message });
            }
            catch (ValuesInvalidException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = mensaje, response = ex.Errors });
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
        }

    }
}
