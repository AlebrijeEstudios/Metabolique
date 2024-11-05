using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Ejercicio;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.ProducesResponseType.Exercise;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/exercises")]
    [RequestTimeout("CustomPolicy")]
    public class ExerciseController : ControllerBase
    {
        private readonly IExercise _ExerciseService;

        public ExerciseController(IExercise ExerciseService)
        {
            _ExerciseService = ExerciseService;

        }

        /// <summary>
        /// This controller returns the exercises performed by the user during the day and obtains the total minutes spent exercising in the last 7 days.
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
        /// <response code="200">Returns two arrays, one with all the exercises performed by the user during the day and 
        /// another with the active minutes during the last 7 days, otherwise, an empty array will be returned for both cases.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetExerciseResponse))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetExercises([FromQuery] Guid accountID, [FromQuery] DateOnly date)
        {

            var infoExercises = await _ExerciseService.GetInfoGeneralExercisesAsync(accountID, date, HttpContext.RequestAborted);

            GetExerciseResponse response = new GetExerciseResponse
            {
                exercises = infoExercises.exercises,
                activeMinutes = infoExercises.activeMinutes,
                mfuStatus = infoExercises.mfuStatus
            };

            return StatusCode(StatusCodes.Status200OK, new { message = response.message,
                                                             exercises = response.exercises, 
                                                             activeMinutes = response.activeMinutes, 
                                                             mfuStatus = response.mfuStatus });
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
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(AddUpdateExercisesResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> AddExercises([FromBody] AddExerciseDto values)
        {
            try
            {
                var exercise = await _ExerciseService.AddExerciseAsync(values, HttpContext.RequestAborted);

                AddUpdateExercisesResponse response = new AddUpdateExercisesResponse
                {
                    exercise = exercise
                };

                return StatusCode(StatusCodes.Status201Created, new { message = response.message, exercise = response.exercise });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (RepeatRegistrationException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (UserNotFoundException ex)
            {

                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, status = response.status });
            }
            catch (ErrorDatabaseException ex)
            {
                ExceptionListMessages response = new ExceptionListMessages
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AddUpdateExercisesResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ApiKeyAuthorizationFilter]
        [HttpPut]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateExercises([FromBody] ExerciseDto values)
        {
            try
            {
                var exercise = await _ExerciseService.UpdateExerciseAsync(values, HttpContext.RequestAborted);

                AddUpdateExercisesResponse response = new AddUpdateExercisesResponse
                {
                    exercise = exercise
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, exercise = response.exercise });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (ExerciseNotFoundException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, status = response.status });
            }
            catch (ErrorDatabaseException ex)
            {
                ExceptionListMessages response = new ExceptionListMessages
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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseMessage))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpDelete("{exerciseID:guid}")]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteExercise(Guid exerciseID)
        {
            try
            {
                var message = await _ExerciseService.DeleteExerciseAsync(exerciseID, HttpContext.RequestAborted);

                ResponseMessage response = new ResponseMessage
                {
                    status = message
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, status = response.status });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (ExerciseNotFoundException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, status = response.status });
            }
        }
    }
}
