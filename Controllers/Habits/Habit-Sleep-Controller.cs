using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Habits_Dtos.Sleep;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType.Habits.SleepHabit;
using AppVidaSana.Services.IServices.IHabits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers.Habits
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/habits-sleep")]
    public class HabitSleepController : ControllerBase
    {
        private readonly ISleepHabit _SleepHabitService;

        public HabitSleepController(ISleepHabit SleepHabitService)
        {
            _SleepHabitService = SleepHabitService;
        }

        /// <summary>
        /// This controller records the hours of sleep and the perception of relaxation.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The dateRegister property must have the following structure:   
        ///     {
        ///        "dateRegister": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///   
        /// </remarks>
        /// <response code="201">Returns a message that the information has been successfully stored.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseSleepHabit))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public IActionResult AddSleepHours([FromBody] SleepHabitDto values)
        {
            try
            {
                var infoHabit = _SleepHabitService.AddSleepHours(values);

                ResponseSleepHabit response = new ResponseSleepHabit
                {
                    sleepHabit = infoHabit
                };

                return StatusCode(StatusCodes.Status201Created, new { message = response.message, sleepHabit = response.sleepHabit });
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
        /// This controller updates information about the hours of sleep.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     From the request body only the following properties are needed:
        ///     {
        ///        "op": "replace",
        ///        "path": {name property},
        ///        "value": {new value (accept null)}
        ///     }
        ///   
        /// </remarks>
        /// <response code="200">Returns a message that the update has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Returns a message indicating that no information about sleeping hours has been found.</response>     
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseSleepHabit))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpPatch]
        [Produces("application/json")]
        public IActionResult UpdateSleepHours([FromQuery] Guid sleepHabitID, [FromBody] JsonPatchDocument values)
        {
            try
            {
                var infoHabit = _SleepHabitService.UpdateSleepHours(sleepHabitID, values);

                ResponseSleepHabit response = new ResponseSleepHabit
                {
                    sleepHabit = infoHabit
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, sleepHabit = response.sleepHabit });
            }

            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (HabitNotFoundException ex)
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
