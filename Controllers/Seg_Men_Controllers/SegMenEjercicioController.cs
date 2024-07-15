using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;
using AppVidaSana.Services.IServices.ISeguimientos_Mensuales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers.Seg_Men_Controllers
{
    [Authorize]
    [EnableCors("ReglasCORS")]
    [ApiController]
    [Route("api/seguimiento-mensual-ejercicio")]

    public class SegMenEjercicioController : ControllerBase
    {
        private readonly ISegMenEjercicio _smejRepo;

        public SegMenEjercicioController(ISegMenEjercicio smejRepo)
        {
            _smejRepo = smejRepo;
        }

        [HttpPost]
        public IActionResult AñadirRespuestas([FromBody] GuardarRespuestasDto gdrdto)
        {
            try
            {
                var res = _smejRepo.GuardarRespuestas(gdrdto);
                return StatusCode(StatusCodes.Status201Created, new { mensaje = "ok", response = res });
            }
            catch (ErrorDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { mensaje = "Hubo un error, intentelo de nuevo", response = ex.Errors });

            }
        }

        [HttpGet]
        public IActionResult RecuperarRespuestas([FromQuery] Guid id, [FromQuery] string mes, [FromQuery] int año)
        {
            try
            {
                RecuperarRespuestasDto res = _smejRepo.RecuperarRespuestas(id, mes, año);
                return StatusCode(StatusCodes.Status200OK, new { mensaje = "ok", response = res });

            }
            catch (UserNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { mensaje = "Hubo un error, intentelo de nuevo", response = ex.Message });

            }
        }
    }
}
