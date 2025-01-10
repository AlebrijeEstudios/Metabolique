using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType.Habits.DrinkHabit;
using AppVidaSana.Services.IServices.IHabits.IHabits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using AppVidaSana.ProducesResponseType;

namespace AppVidaSana.Controllers.Habits
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/habits-drink")]
    public class HabitDrinkController : ControllerBase
    {
        private readonly IDrinkHabit _DrinkHabitService;

        public HabitDrinkController(IDrinkHabit DrinkHabitService)
        {
            _DrinkHabitService = DrinkHabitService;
        }

        /// <summary>
        /// This controller adds the amount consumed by the user.
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
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDrinkHabit))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExpiredTokenException))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public IActionResult AddDrinksConsumed([FromBody] DrinkHabitDto values)
        {
            try
            {
                var infoHabit = _DrinkHabitService.AddDrinksConsumed(values);

                ResponseDrinkHabit response = new ResponseDrinkHabit
                {
                    drinkConsumed = infoHabit
                };

                return StatusCode(StatusCodes.Status201Created, new { message = response.message, drinkConsumed = response.drinkConsumed });
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
        /// This controller updates the beverages consumed by the user.
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
        /// <response code="401">Returns a message indicating that the token has expired.</response>
        /// <response code="404">Returns a message indicating that no records were found for certain beverages consumed.</response>     
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDrinkHabit))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExpiredTokenException))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpPatch]
        [Produces("application/json")]
        public IActionResult UpdateDrinksConsumed([FromQuery] Guid drinkHabitID, [FromBody] JsonPatchDocument values)
        {
            try
            {
                var infoHabit = _DrinkHabitService.UpdateDrinksConsumed(drinkHabitID, values);

                ResponseDrinkHabit response = new ResponseDrinkHabit
                {
                    drinkConsumed = infoHabit
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, drinkConsumed = response.drinkConsumed });
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
