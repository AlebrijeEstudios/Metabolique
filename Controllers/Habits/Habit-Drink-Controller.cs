using AppVidaSana.Api;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Services.IServices.IHabits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.ProducesResponseType.Habits;
using AppVidaSana.Exceptions.Habits;
using AppVidaSana.Exceptions;
using AppVidaSana.Models.Dtos.Habits_Dtos.Drink;

namespace AppVidaSana.Controllers.Habits
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/habits-drink")]
    [EnableRateLimiting("sliding")]
    public class HabitDrinkController : ControllerBase
    {
        private readonly IDrinkHabit _DrinkHabitService;

        public HabitDrinkController(IDrinkHabit DrinkHabitService)
        {
            _DrinkHabitService = DrinkHabitService;
        }

        /// <summary>
        /// This controller adds the beverages consumed by the user and returns the beverages consumed by the user.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The drinkDateHabit property must have the following structure:   
        ///     {
        ///        "drinkDateHabit": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///   
        /// </remarks>
        /// <response code="201">Returns a message that the information has been successfully stored. The information is stored in the attribute called 'response'.</response>
        /// <response code="400">Returns a message that the requested action could not be performed. The information is stored in the attribute called 'response'.</response>
        /// <response code="404">Return an error message if the user is not found. The information is stored in the attribute called 'response'.</response>
        /// <response code="409">Returns a series of messages indicating that some values are invalid. The information is stored in the attribute called 'response'.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ReturnAddUpdateDeleteDrinkConsumed))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public IActionResult AddDrinksConsumed([FromBody] DrinksConsumedDto drinksConsumed)
        {
            try
            {
                List<GetDrinksConsumedDto> res = _DrinkHabitService.AddDrinksConsumed(drinksConsumed);

                ReturnAddUpdateDeleteDrinkConsumed response = new ReturnAddUpdateDeleteDrinkConsumed
                {
                    drinksConsumed = res
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
            catch (RepeatRegistrationException ex)
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
        /// This controller updates the beverages consumed by the user.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful. The information is stored in the attribute called 'response'.</response>
        /// <response code="400">Returns a message that the requested action could not be performed. The information is stored in the attribute called 'response'.</response>
        /// <response code="404">Returns a message indicating that no records were found for certain beverages consumed. The information is stored in the attribute called 'response'.</response>     
        /// <response code="409">Returns a series of messages indicating that some values are invalid. The information is stored in the attribute called 'response'.</response>
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnAddUpdateDeleteDrinkConsumed))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ReturnExceptionList))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpPut]
        [Produces("application/json")]
        public IActionResult UpdateDrinksConsumed([FromBody] UpdateDrinksConsumedDto values)
        {
            try
            {
                List<GetDrinksConsumedDto> res = _DrinkHabitService.UpdateDrinksConsumed(values);

                ReturnAddUpdateDeleteDrinkConsumed response = new ReturnAddUpdateDeleteDrinkConsumed
                {
                    drinksConsumed = res
                };

                return StatusCode(StatusCodes.Status200OK, new { response });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { response });
            }
            catch (HabitNotFoundException ex)
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
        /// This controller deletes a record containing certain beverages consumed.
        /// </summary>
        /// <response code="200">Returns a message that the elimination has been successful. The information is stored in the attribute called 'response'.</response>
        /// <response code="400">Returns a message that the requested action could not be performed. The information is stored in the attribute called 'response'.</response>
        /// <response code="404">Returns a message indicating that the record with the specified consumed beverages does not exist in the DrinkHabit table. The information is stored in the attribute called 'response'.</response>     
        /// <response code="429">Returns a message indicating that the limit of allowed requests has been reached.</response>       
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnAddUpdateDeleteDrinkConsumed))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ReturnExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RateLimiting))]
        [ApiKeyAuthorizationFilter]
        [HttpDelete("{id:guid}")]
        [Produces("application/json")]
        public IActionResult DeleteDrinkConsumed(Guid id)
        {
            try
            {
                List<GetDrinksConsumedDto> res = _DrinkHabitService.DeleteDrinksConsumed(id);

                ReturnAddUpdateDeleteDrinkConsumed response = new ReturnAddUpdateDeleteDrinkConsumed
                {
                    drinksConsumed = res
                };

                return StatusCode(StatusCodes.Status200OK, new { response });
            }
            catch (UnstoredValuesException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { response });
            }
            catch (HabitNotFoundException ex)
            {
                ReturnExceptionMessage response = new ReturnExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { response });
            }
        }
    }
}
