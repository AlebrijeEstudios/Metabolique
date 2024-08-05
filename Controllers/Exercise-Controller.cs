using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Exceptions.Ejercicio;
using AppVidaSana.Api;
using AppVidaSana.Models;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.Models.Dtos.Graphics_Dtos;
using AppVidaSana.ProducesResponseType.Exercise;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Exceptions;
using AppVidaSana.ProducesResponseType.Exercise.MFUsExercise;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AppVidaSana.Controllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/exercises")]
    [EnableRateLimiting("sliding")]
    public class ExerciseController : ControllerBase
    {
        private readonly IExercise _ExerciseService;

        public ExerciseController(IExercise ExerciseService)
        {
            _ExerciseService = ExerciseService;

        }

        /// <summary>
        /// This controller returns the exercises performed by the user during the day.
        /// </summary>
        /// <response code="200">Returns an array with all the exercises performed by the user during the day or an empty array.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnGetExercise))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetExercises([FromQuery] Guid id, [FromQuery] DateOnly date)
        {

            List<ExerciseListDto> exercises = _ExerciseService.GetExercises(id, date);

            ReturnGetExercise response = new ReturnGetExercise
            {
                exercises = exercises
            };

            return StatusCode(StatusCodes.Status200OK, new { message = response.message, exercises = response.exercises });
        }


        /// <summary>
        /// This controller obtains the total minutes spent exercising in the last 7 days.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The dateExercise property must have the following structure:   
        ///     {
        ///        "dateExercise": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns an array with the last 7 days including the total minutes consumed or an empty array.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnGetValuesGraphic))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("minutes-consumed")]
        [Produces("application/json")]
        public IActionResult GetValuesGraphic([FromQuery] Guid id, [FromQuery] DateOnly date)
        {

            List<GraphicsValuesExerciseDto> values = _ExerciseService.ValuesGraphicExercises(id,date);

            ReturnGetValuesGraphic response = new ReturnGetValuesGraphic
            {
                timeSpentsforDay = values
            };

            return StatusCode(StatusCodes.Status200OK, new { message = response.message, timeSpentsforDay = response.timeSpentsforDay });
            
        }

        /// <summary>
        /// This controller adds the exercises that the user does during the day and returns the exercises performed by the user during the day.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The dateExercise property must have the following structure:   
        ///     {
        ///        "dateExercise": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///   
        /// </remarks>
        /// <response code="201">Returns a message that the information has been successfully stored.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Return an error message if the user is not found.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnAddUpdateDeleteExercises))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public IActionResult AddExercises([FromBody] AddExerciseDto exercise)
        {
            try
            {
                List<ExerciseListDto> exercises = _ExerciseService.AddExercises(exercise);

                ReturnAddUpdateDeleteExercises response = new ReturnAddUpdateDeleteExercises
                {
                    exercises = exercises
                };

                return StatusCode(StatusCodes.Status201Created, new { message = response.message, exercises = response.exercises });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (RepeatRegistrationException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (UserNotFoundException ex)
            {

                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, status = response.status });
            }
            catch (ErrorDatabaseException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { message = response.message, status = response.status });

            }
        }

        /// <summary>
        /// This controller updates the exercises.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Returns a message indicating that no record(s) were found for a certain exercise.</response>     
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnAddUpdateDeleteExercises))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPut]
        [Produces("application/json")]
        public IActionResult UpdateExercises([FromBody] ExerciseListDto listExercises)
        {
            try
            {
                List<ExerciseListDto> exercises = _ExerciseService.UpdateExercises(listExercises);

                ReturnAddUpdateDeleteExercises response = new ReturnAddUpdateDeleteExercises
                {
                    exercises = exercises
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, exercises = response.exercises });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (ExerciseNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, status = response.status });
            }
            catch (ErrorDatabaseException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { message = response.message, status = response.status });
            }
        }

        /// <summary>
        /// This controller deletes a registered fiscal year.
        /// </summary>
        /// <response code="200">Returns a message that the elimination has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Returns a message indicating that an exercise does not exist in the Exercises table.</response>     
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>       
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnAddUpdateDeleteExercises))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpDelete("{id:guid}")]
        [Produces("application/json")]
        public IActionResult DeleteExercise(Guid id)
        {
            try
            {
                List<ExerciseListDto> exercises = _ExerciseService.DeleteExercise(id);

                ReturnAddUpdateDeleteExercises response = new ReturnAddUpdateDeleteExercises
                {
                    exercises = exercises
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, exercises = response.exercises });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (ExerciseNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, status = response.status });
            }
        }
    }
}
