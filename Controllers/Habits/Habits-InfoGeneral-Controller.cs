using AppVidaSana.Api;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.ProducesResponseType.Habits;
using AppVidaSana.Services.IServices.IHabits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers.Habits
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/habits")]
    public class HabitsInfoGeneralController : ControllerBase
    {
        private readonly IHabitsGeneral _HabitsInfoService;

        public HabitsInfoGeneralController(IHabitsGeneral HabitsInfoService)
        {
            _HabitsInfoService = HabitsInfoService;
        }

        /// <summary>
        /// This controller contains all the information in the habits section and returns the hours of sleep in the last 7 days..
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The sleepDateHabit,drugDateHabit and drinkDateHabit property must have the following structure:   
        ///     {
        ///        "drinkDateHabit": "0000-00-00" (YEAR-MOUNTH-DAY),
        ///        "sleepDateHabit": "0000-00-00" (YEAR-MOUNTH-DAY),
        ///        "drugDateHabit": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">Returns all the information from the Habits section for a given day and the hours of sleep for the last 7 days.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ReturnHabitsInfo))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetHabitsInfoGeneralAsync([FromQuery] Guid accountID, [FromQuery] DateOnly date)
        {

            var infoGeneralHabits = await _HabitsInfoService.GetInfoGeneralHabitsAsync(accountID, date, HttpContext.RequestAborted);

            ReturnHabitsInfo response = new ReturnHabitsInfo
            {
                drinkConsumed = infoGeneralHabits.drinkConsumed,
                hoursSleepConsumed = infoGeneralHabits.hoursSleepConsumed,
                drugsConsumed = infoGeneralHabits.drugsConsumed,
                hoursSleep = infoGeneralHabits.hoursSleep,
                mfuStatus = infoGeneralHabits.mfuStatus
            };

            return StatusCode(StatusCodes.Status200OK, new
            {
                message = response.message,
                drinkConsumed = response.drinkConsumed,
                hoursSleepConsumed = response.hoursSleepConsumed,
                drugsConsumed = response.drugsConsumed,
                hoursSleep = response.hoursSleep,
                mfuStatus = response.mfuStatus
            });
        }
    }
}
