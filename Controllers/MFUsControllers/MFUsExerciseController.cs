using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Seguimientos_Mensuales_Dto.Ejercicio_Dtos;
using AppVidaSana.Services.IServices.ISeguimientos_Mensuales;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers.Seg_Men_Controllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/monthly-exercise-monitoring")]

    public class MFUsExerciseController : ControllerBase
    {
        private readonly IMFUsExercise _MFUsExcerciseService;
        private string mensaje = "Hubo un error, intentelo de nuevo.";

        public MFUsExerciseController(IMFUsExercise MFUsExcerciseService)
        {
            _MFUsExcerciseService = MFUsExcerciseService;
        }

        [HttpPost]
        public IActionResult AddExcercise([FromBody] SaveResponsesDto responses)
        {
            try
            {
                var res = _MFUsExcerciseService.SaveAnswers(responses);
                return StatusCode(StatusCodes.Status201Created, new { message = "ok", response = res });
            }
            catch (ErrorDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = ex.Errors });
            }
        }

        [HttpGet]
        public IActionResult RetrieveResponses([FromQuery] Guid id, [FromQuery] string month, [FromQuery] int year)
        {
            try
            {
                RetrieveResponsesDto res = _MFUsExcerciseService.RetrieveAnswers(id, month, year);
                return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = res });
            }
            catch (UserNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = mensaje, response = ex.Message });
            }
        }
    }
}
