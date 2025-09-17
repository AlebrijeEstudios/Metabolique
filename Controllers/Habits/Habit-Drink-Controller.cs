using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.ProducesResponseType.Habits.DrinkHabit;
using AppVidaSana.Services.IServices.IHabits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.ProducesResponseType.ResponseOperationsFilters.ApiResponsesAttribute;

namespace AppVidaSana.Controllers.Habits
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("App - HabitsDrink")]
    [ApiExplorerSettings(GroupName = "app")]
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
        [CommonApiResponse]
        [BadRequestApiResponse]
        [UnauthorizedApiResponse]
        [ConflictApiResponse]
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ResponseDrinkHabit))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("write")]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> AddDrinksConsumedAsync([FromBody] DrinkHabitDto values)
        {
            try
            {
                var infoHabit = await _DrinkHabitService.AddDrinksConsumedAsync(values, HttpContext.RequestAborted);

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
        /// <response code="404">Returns a message indicating that no records were found for certain beverages consumed.</response>
        [CommonApiResponse]
        [BadRequestApiResponse]
        [UnauthorizedApiResponse]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ResponseDrinkHabit))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("write")]
        [HttpPatch]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateDrinksConsumedAsync([FromQuery] Guid drinkHabitID, [FromBody] JsonPatchDocument values)
        {
            try
            {
                var infoHabit = await _DrinkHabitService.UpdateDrinksConsumedAsync(drinkHabitID, values, HttpContext.RequestAborted);

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