using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models;
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
    [Route("api/users")]
    public class CuentaController : Controller
    {
        private readonly ICuenta _uRepo;
        private readonly IMapper _mapper;

        public CuentaController(ICuenta uRepo, IMapper mapper)
        {
            _uRepo = uRepo;
            _mapper = mapper;
        }

        [HttpGet("{id:guid}")]
        public IActionResult GetUser(Guid id)
        {
            try
            {
                Cuenta user = _uRepo.GetUser(id);

                UserInfoDto userDto = _mapper.Map<UserInfoDto>(user);

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = userDto });

            }
            catch (UserNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Hubo un error, intentelo de nuevo", response = ex.Message });

            }
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult CreateUser([FromBody] RegisterUserDto user)
        {
            try
            {
               try
               {
                    var res = _uRepo.CreateUser(user);
                    return StatusCode(StatusCodes.Status201Created, new { mensaje = "ok", response = res });

               }catch(PasswordInvalidException ex)
               {
                    return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Hubo un error, intentelo de nuevo", response = ex.Message });
               }
            }
            catch (ErrorDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Hubo un error, intentelo de nuevo", response = ex.Errors });

            }
        }

        [HttpPut("{id:guid}")]
        public IActionResult UpdateUser(Guid id, [FromBody] UserInfoDto userdto)
        {
            try
            {
                try
                {
                    var res = _uRepo.UpdateUser(id, userdto);
                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = res });

                }catch(UserNotFoundException ex)
                {
                    return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Hubo un error, intentelo de nuevo", response = ex.Message });

                }
            }
            catch (ErrorDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Hubo un error, intentelo de nuevo", response = ex.Errors });

            }
        }

        [HttpDelete("{id:guid}")]
        public IActionResult DeleteUser(Guid id)
        {
            try
            {
                var res = _uRepo.DeleteUser(id);
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = res });
            }
            catch (UserNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Error al realizar la eliminación", response = ex.Message });

            }
        }

    }
}
