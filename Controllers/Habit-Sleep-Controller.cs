using AppVidaSana.Api;
using AppVidaSana.Exceptions.Cuenta_Perfil;
using AppVidaSana.Models.Dtos.Account_Profile_Dtos;
using AppVidaSana.ProducesReponseType;
using AppVidaSana.ProducesResponseType.Account;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Services;
using AppVidaSana.Services.IServices;
using AppVidaSana.Services.IServices.IHabits;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.Models.Dtos.Habits_Dtos;
using AppVidaSana.ProducesResponseType.Habits;
using AppVidaSana.Exceptions.Habits;

namespace AppVidaSana.Controllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/habits-sleep")]
    [EnableRateLimiting("sliding")]
    public class Habit_SleepController : ControllerBase
    {
        private readonly ISleepHabit _SleepHabitService;

        public Habit_SleepController(ISleepHabit SleepHabitService)
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








    }
}
