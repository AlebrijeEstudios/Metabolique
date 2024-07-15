using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Exceptions.Ejercicio;

namespace AppVidaSana.Controllers
{
    [Authorize]
    [EnableCors("ReglasCORS")]
    [ApiController]
    [Route("api/users-ejercicios")]
    public class EjercicioController : ControllerBase
    {
        private readonly IEjercicio _ejRepo;

        public EjercicioController(IEjercicio ejRepo)
        {
            _ejRepo = ejRepo;
        }

        [HttpGet]
        public IActionResult ObtenerEjercicios([FromQuery] Guid id, [FromQuery] DateOnly fecha)
        {
            try
            {
                List<ListaEjerciciosDto> ejercicios = _ejRepo.ObtenerEjercicios(id, fecha);

                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = ejercicios });

            }
            catch (EjercicioNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Hubo un error, intentelo de nuevo", response = ex.Message });

            }
        }

        [HttpPost]
        public IActionResult AñadirEjercicios([FromBody] AñadirEjercicioDto añdto)
        {
            try
            {

                var res = _ejRepo.AñadirEjercicio(añdto);
                return StatusCode(StatusCodes.Status201Created, new { mensaje = "ok", response = res });

            }
            catch (ErrorDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Hubo un error, intentelo de nuevo", response = ex.Errors });

            }
        }

        [HttpPut("{id:guid}")]
        public IActionResult ActualizarEjercicio(Guid id, [FromBody] ListaEjerciciosDto listadto)
        {
            try
            {
                try
                {
                    var res = _ejRepo.ActualizarEjercicio(id, listadto);
                    return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = res });

                }
                catch (EjercicioNotFoundException ex)
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
        public IActionResult EliminarEjercicio(Guid id)
        {
            try
            {
                var res = _ejRepo.EliminarEjercicio(id);
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = res });
            }
            catch (EjercicioNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Error al realizar la eliminación", response = ex.Message });

            }
        }
    }
}
