using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Exceptions.Ejercicio;
using AppVidaSana.Api;

namespace AppVidaSana.Controllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/exercises")]
    public class ExerciseController : ControllerBase
    {
        private readonly IExercise _ExerciseService;
        private string mensaje = "Hubo un error, intentelo de nuevo.";

        public ExerciseController(IExercise ExerciseService)
        {
            _ExerciseService = ExerciseService;
        
        }
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        public IActionResult GetExercises([FromQuery] Guid id, [FromQuery] DateOnly date)
        {
            try
            {
                List<ExerciseListDto> exercises = _ExerciseService.GetExercises(id, date);
                return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = exercises });
            }
            catch (ExerciseNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = mensaje, response = ex.Message });
            }
        }

        [ApiKeyAuthorizationFilter]
        [HttpPost]
        public IActionResult AddExercises([FromBody] AddExerciseDto exercise)
        {
            try
            {
                var res = _ExerciseService.AddExercises(exercise);
                return StatusCode(StatusCodes.Status201Created, new { message = "ok", response = res });
            }
            catch (ErrorDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = ex.Errors });

            }
        }

        
        [ApiKeyAuthorizationFilter]
        [HttpPut("{id:guid}")]
        public IActionResult UpdateExercises(Guid id, [FromBody] ExerciseListDto listExercises)
        {
            try
            {

                var res = _ExerciseService.UpdateExercises(id, listExercises);
                return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = res });
            }
            catch (ExerciseNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = mensaje, response = ex.Message });
            }
            catch (ErrorDatabaseException ex)
            {
                return StatusCode(StatusCodes.Status400BadRequest, new { message = mensaje, response = ex.Errors });
            }
        }
        
        [ApiKeyAuthorizationFilter]
        [HttpDelete("{id:guid}")]
        public IActionResult DeleteExercise(Guid id)
        {
            try
            {
                var res = _ExerciseService.DeleteExercise(id);
                return StatusCode(StatusCodes.Status200OK, new { message = "ok", response = res });
            }
            catch (ExerciseNotFoundException ex)
            {
                return StatusCode(StatusCodes.Status404NotFound, new { message = mensaje, response = ex.Message });
            }
        }
    }
}
