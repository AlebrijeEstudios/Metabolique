using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Models.Dtos.Ejercicio_Dtos;
using AppVidaSana.Exceptions.Ejercicio;
using AppVidaSana.Api;
using AppVidaSana.Models;
using AppVidaSana.Exceptions.Account_Profile;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.Models.Dtos.Graphics_Dtos;
using AppVidaSana.ProducesResponseType.Exercise;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.ProducesResponseType;

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
        /// <response code="200">Returns an array with all the exercises performed by the user during the day. The information is stored in the attribute called 'response'.</response>
        /// <response code="404">Returns a message indicating that no records were found for that date. The information is stored in the attribute called 'response'.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnGetExercises))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetExercises([FromQuery] Guid id, [FromQuery] DateOnly date)
        {
            try
            {
                List<ExerciseListDto> exercises = _ExerciseService.GetExercises(id, date);

                ReturnGetExercises response = new ReturnGetExercises
                {
                    exercises = exercises
                };

                return StatusCode(StatusCodes.Status200OK, new { response });
            }
            catch (ExerciseNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
            }
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
        /// <response code="200">Returns an array with the last 7 days including total minutes spent. The information is stored in the attribute called 'response'.</response>
        /// <response code="404">Returns a message indicating that there are no records associated with that date. The information is stored in the attribute called 'response'.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnGetValuesGraphic))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("minutes-consumed")]
        [Produces("application/json")]
        public IActionResult GetValuesGraphic([FromQuery] Guid id, [FromQuery] DateOnly date)
        {
            try
            {
                List<GExerciseDto> values = _ExerciseService.ValuesGraphicExercises(id,date);

                ReturnGetValuesGraphic response = new ReturnGetValuesGraphic
                {
                    timeSpentsforDay = values
                };

                return StatusCode(StatusCodes.Status200OK, new { response });
            }
            catch (ExerciseNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                }; 

                return StatusCode(StatusCodes.Status404NotFound, new { response });
            }
        }

        /// <summary>
        /// This controller adds the exercises that the user does during the day.
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
        /// <response code="201">Returns a message that the information has been successfully stored. The information is stored in the attribute called 'response'.</response>
        /// <response code="400">Returns a message that the requested action could not be performed. The information is stored in the attribute called 'response'.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid. The information is stored in the attribute called 'response'.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(Return_Add_Update_Delete_Exercises))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public IActionResult AddExercises([FromBody] AddExerciseDto exercise)
        {
            try
            {
                var res = _ExerciseService.AddExercises(exercise);

                Return_Add_Update_Delete_Exercises response = new Return_Add_Update_Delete_Exercises
                {
                    status = res
                };

                return StatusCode(StatusCodes.Status201Created, new { response });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { response });
            }
            catch (ErrorDatabaseException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { response });

            }
        }

        /// <summary>
        /// This controller updates the exercises.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful. The information is stored in the attribute called 'response'.</response>
        /// <response code="400">Returns a message that the requested action could not be performed. The information is stored in the attribute called 'response'.</response>
        /// <response code="404">Returns a message indicating that no record(s) were found for a certain exercise. The information is stored in the attribute called 'response'.</response>     
        /// <response code="409">Returns a series of messages indicating that some values are invalid. The information is stored in the attribute called 'response'.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Return_Add_Update_Delete_Exercises))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPut("{id:guid}")]
        [Produces("application/json")]
        public IActionResult UpdateExercises(Guid id, [FromBody] ExerciseListDto listExercises)
        {
            try
            {
                var res = _ExerciseService.UpdateExercises(id, listExercises);

                Return_Add_Update_Delete_Exercises response = new Return_Add_Update_Delete_Exercises
                {
                    status = res
                };

                return StatusCode(StatusCodes.Status200OK, new { response });
            }
            catch (ExerciseNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { response });
            }
            catch (ErrorDatabaseException ex)
            {
                ReturnExceptionList response = new ReturnExceptionList
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { response });
            }
        }

        /// <summary>
        /// This controller deletes a registered fiscal year.
        /// </summary>
        /// <response code="200">Returns a message that the elimination has been successful. The information is stored in the attribute called 'response'.</response>
        /// <response code="400">Returns a message that the requested action could not be performed. The information is stored in the attribute called 'response'.</response>
        /// <response code="404">Returns a message indicating that an exercise does not exist in the Exercises table. The information is stored in the attribute called 'response'.</response>     
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>       
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Return_Add_Update_Delete_Exercises))]
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
                var res = _ExerciseService.DeleteExercise(id);

                Return_Add_Update_Delete_Exercises response = new Return_Add_Update_Delete_Exercises
                {
                    status = res
                };

                return StatusCode(StatusCodes.Status200OK, new { response });
            }
            catch (ExerciseNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { response });
            }
        }
    }
}
