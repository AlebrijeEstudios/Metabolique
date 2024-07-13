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
    [EnableCors("ReglasCORS")]
    [ApiController]
    [Route("api/users/profile")]
    public class PerfilController : Controller
    {
        private readonly IPerfil _upRepo;

        public PerfilController(IPerfil upRepo)
        {
            _upRepo = upRepo;
        }

        [HttpPost("{id:guid}")]
        public IActionResult CreateProfile(Guid id, [FromBody] ProfileUserDto profile)
        {
            try
            {
                try
                {
                    var res = _upRepo.CreateProfile(id, profile);
                    return StatusCode(StatusCodes.Status201Created, new { mensaje = "ok", response = res });
                }
                catch (UserNotFoundException ex)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Hubo un error, intentelo de nuevo", response = ex.Message });

                }
            }
            catch (ErrorDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Error", response = ex.Errors });
            }
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetProfile(Guid id)
        {
            try
            {
                ProfileUserDto profile = _upRepo.GetProfile(id);
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = profile });

            }
            catch (UserNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Hubo un error, intentelo de nuevo", response = ex.Message });

            }
        }

        [HttpPut("{id:guid}")]
        public IActionResult UpdateProfile(Guid id, [FromBody] ProfileUserDto profiledto)
        {
            try
            {
                try
                {
                    var res = _upRepo.UpdateProfile(id, profiledto);
                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = res });
                }
                catch (UserNotFoundException ex)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "ok", response = ex.Message });
                }
            }catch(ErrorDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "ok", response = ex.Errors });
            }
        }

    }
}
