using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drugs;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.ProducesResponseType.Habits.DrugsHabit;
using AppVidaSana.Services.IServices.IHabits.IHabits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace AppVidaSana.Controllers.Habits
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/habits-drugs")]
    [EnableRateLimiting("sliding")]
    public class HabitDrugsController : ControllerBase
    {
        private readonly IDrugsHabit _DrugsHabitService;

        public HabitDrugsController(IDrugsHabit DrugsHabitService)
        {
            _DrugsHabitService = DrugsHabitService;
        }

        /// <summary>
        /// This controller adds the cigars consumed by the user.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The drugsDateHabit property must have the following structure:   
        ///     {
        ///        "drugsDateHabit": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///   
        /// </remarks>
        /// <response code="201">Returns a message that the information has been successfully stored.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Return an error message if the user is not found.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnAddUpdateDrugsConsumed))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public IActionResult AddDrugsConsumed([FromBody] DrugsConsumedDto drugsConsumed)
        {
            try
            {
                var res = _DrugsHabitService.AddDrugsConsumed(drugsConsumed);

                ReturnAddUpdateDrugsConsumed response = new ReturnAddUpdateDrugsConsumed
                {
                    drugsConsumed = res
                };

                return StatusCode(StatusCodes.Status201Created, new { message = response.message, status = response.drugsConsumed });
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
        /// This controller updates the cigars consumed by the user.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Returns a message indicating that no records have been found for a certain number of cigarettes consumed.</response>     
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnAddUpdateDrugsConsumed))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPut]
        [Produces("application/json")]
        public IActionResult UpdateDrugsConsumed([FromBody] UpdateDrugsConsumedDto values)
        {
            try
            {
                var res = _DrugsHabitService.UpdateDrugsConsumed(values);

                ReturnAddUpdateDrugsConsumed response = new ReturnAddUpdateDrugsConsumed
                {
                    drugsConsumed = res
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, status = response.drugsConsumed });
            }
            
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (HabitNotFoundException ex)
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
        /// This controller deletes a record containing certain cigars consumed.
        /// </summary>
        /// <response code="200">Returns a message that the elimination has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="404">Returns a message indicating that the record with the cigarettes consumed does not exist in the DrugsHabit table.</response>     
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>       
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnAddUpdateDrugsConsumed))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpDelete("{id:guid}")]
        [Produces("application/json")]
        public IActionResult DeleteDrugsConsumed(Guid id)
        {
            try
            {
                var res = _DrugsHabitService.DeleteDrugsConsumed(id);

                ReturnDeleteDrugsConsumed response = new ReturnDeleteDrugsConsumed
                {
                    status = res
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, status = response.status });
            } 
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (HabitNotFoundException ex)
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
