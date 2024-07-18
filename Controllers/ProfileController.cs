using AppVidaSana.Api;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Cuenta_Perfil_Dtos;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/account/profile")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfile _ProfileService;
        private string mensaje = "Hubo un error, intentelo de nuevo.";

        public ProfileController(IProfile ProfileService)
        {
            _ProfileService = ProfileService;
        }

        [ApiKeyAuthorizationFilter]
        [HttpPost("{id:guid}")]
        public IActionResult CreateProfile(Guid id, [FromBody] ProfileUserDto profile)
        {
            try
            {
                var res = _ProfileService.CreateProfile(id, profile);
                return StatusCode(StatusCodes.Status201Created, new { message = "ok", response = res });
            }
            catch (UserNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = mensaje, response = ex.Message });

            }
            catch (ErrorDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = ex.Errors });
            }
        }

        [ApiKeyAuthorizationFilter]
        [HttpGet("{id:guid}")]
        public IActionResult GetProfile(Guid id)
        {
            try
            {
                ProfileUserDto profile = _ProfileService.GetProfile(id);
                return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = profile });
            }
            catch (UserNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = mensaje, response = ex.Message });
            }
        }

        [ApiKeyAuthorizationFilter]
        [HttpPut("{id:guid}")]
        public IActionResult UpdateProfile(Guid id, [FromBody] ProfileUserDto profile)
        {
            try
            {
                var res = _ProfileService.UpdateProfile(id, profile);
                return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = res });
            }
            catch (UserNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = mensaje, response = ex.Message });
            }
            catch(ErrorDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = ex.Errors });
            }
        }

    }
}
