using AppVidaSana.Api;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Services.IServices.IHabits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.Models.Dtos.Habits_Dtos;
using AppVidaSana.ProducesResponseType.Habits;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Exceptions;
using AppVidaSana.ProducesResponseType.Exercise;

namespace AppVidaSana.Controllers.Habits
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/habits-sleep")]
    [EnableRateLimiting("sliding")]
    public class HabitSleepController : ControllerBase
    {
        private readonly ISleepHabit _SleepHabitService;

        public HabitSleepController(ISleepHabit SleepHabitService)
        {
            _SleepHabitService = SleepHabitService;
        }

        /// <summary>
        /// This controller returns the hours of sleep.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The sleepDateHabit property must have the following structure:   
        ///     {
        ///        "sleepDateHabit": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns sleeping hours information if found. The information is stored in the attribute called 'response'.</response>
        /// <response code="404">Return an error message if the information is not found. The information is stored in the attribute called 'response'.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnGetSleepingHours))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public IActionResult GetSleepingHours([FromQuery] Guid id, [FromQuery] DateOnly date)
        {
            try
            {
                List<GetSleepingHoursDto> info = _SleepHabitService.GetSleepingHours(id, date);

                ReturnGetSleepingHours response = new ReturnGetSleepingHours
                {
                    hoursSleep = info
                };

                return StatusCode(StatusCodes.Status200OK, new { response });
            }
            catch (HoursSleepNotFoundException ex)
            {

                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
            }
        }

        /// <summary>
        /// This controller adds the user's sleep hours.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The sleepDateHabit property must have the following structure:   
        ///     {
        ///        "sleepDateHabit": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///   
        /// </remarks>
        /// <response code="201">Returns a message that the information has been successfully stored. The information is stored in the attribute called 'response'.</response>
        /// <response code="400">Returns a message that the requested action could not be performed. The information is stored in the attribute called 'response'.</response>
        /// <response code="404">Return an error message if the user is not found. The information is stored in the attribute called 'response'.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid. The information is stored in the attribute called 'response'.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnAddUpdateDeleteSleepHours))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public IActionResult AddSleepHours([FromBody] SleepingHoursDto sleepingHours)
        {
            try
            {
                var res = _SleepHabitService.AddSleepHours(sleepingHours);

                ReturnAddUpdateDeleteSleepHours response = new ReturnAddUpdateDeleteSleepHours
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
            catch (UserNotFoundException ex)
            {

                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
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
        /// This controller updates the user's sleep hours.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful. The information is stored in the attribute called 'response'.</response>
        /// <response code="400">Returns a message that the requested action could not be performed. The information is stored in the attribute called 'response'.</response>
        /// <response code="404">Returns a message indicating that no records of certain hours of sleep have been found. The information is stored in the attribute called 'response'.</response>     
        /// <response code="409">Returns a series of messages indicating that some values are invalid. The information is stored in the attribute called 'response'.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnAddUpdateDeleteSleepHours))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPut]
        [Produces("application/json")]
        public IActionResult UpdateSleepHours([FromBody] UpdateSleepingHoursDto values)
        {
            try
            {
                var res = _SleepHabitService.UpdateSleepHours(values);

                ReturnAddUpdateDeleteSleepHours response = new ReturnAddUpdateDeleteSleepHours
                {
                    status = res
                };

                return StatusCode(StatusCodes.Status200OK, new { response });
            }
            catch (HabitNotFoundException ex)
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
        /// This controller deletes a log containing certain hours of sleep.
        /// </summary>
        /// <response code="200">Returns a message that the elimination has been successful. The information is stored in the attribute called 'response'.</response>
        /// <response code="400">Returns a message that the requested action could not be performed. The information is stored in the attribute called 'response'.</response>
        /// <response code="404">Returns a message indicating that the record with the specified sleep hours does not exist in the SleepHabit table. The information is stored in the attribute called 'response'.</response>     
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>       
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnAddUpdateDeleteExercises))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpDelete("{id:guid}")]
        [Produces("application/json")]
        public IActionResult DeleteSleepHours(Guid id)
        {
            try
            {
                var res = _SleepHabitService.DeleteSleepHours(id);

                ReturnAddUpdateDeleteSleepHours response = new ReturnAddUpdateDeleteSleepHours
                {
                    status = res
                };

                return StatusCode(StatusCodes.Status200OK, new { response });
            }
            catch (HabitNotFoundException ex)
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
