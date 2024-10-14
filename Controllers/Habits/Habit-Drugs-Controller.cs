using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drugs;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType.Habits.DrugsHabit;
using AppVidaSana.Services.IServices.IHabits.IHabits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers.Habits
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/habits-drugs")]
    public class HabitDrugsController : ControllerBase
    {
        private readonly IDrugsHabit _DrugsHabitService;

        public HabitDrugsController(IDrugsHabit DrugsHabitService)
        {
            _DrugsHabitService = DrugsHabitService;
        }

        /// <summary>
        /// This controller adds the cigars consumed by the user and the user's sleep hours.
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
        /// <response code="404">Return an error message if the user is not found.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDrugsHabit))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionDB))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public IActionResult AddDrugsConsumed([FromBody] DrugsHabitDto values)
        {
            try
            {
                var infoHabit = _DrugsHabitService.AddDrugsConsumed(values);

                ResponseDrugsHabit response = new ResponseDrugsHabit
                {
                    drugsHabit = infoHabit
                };

                return StatusCode(StatusCodes.Status201Created, new { message = response.message, drugsHabit = response.drugsHabit });
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
                ExceptionDB response = new ExceptionDB
                {
                    status = ex.Errors
                };

                return StatusCode(StatusCodes.Status409Conflict, new { message = response.message, status = response.status });

            }
        }

        /// <summary>
        /// This controller updates drug consumption records.
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
        /// <response code="404">Returns a message indicating that there is no information on drug use.</response>     
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDrugsHabit))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpPatch]
        [Produces("application/json")]
        public IActionResult UpdateDrugsConsumed([FromQuery] Guid drugsHabitID, [FromBody] JsonPatchDocument values)
        {
            try
            {
                var infoHabit = _DrugsHabitService.UpdateDrugsConsumed(drugsHabitID, values);

                ResponseDrugsHabit response = new ResponseDrugsHabit
                {
                    drugsHabit = infoHabit
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, drugsHabit = response.drugsHabit });
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
