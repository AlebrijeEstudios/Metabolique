using AppVidaSana.Services.IServices.IAdminWeb;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.ProducesResponseType.AdminWeb.Habit;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;

namespace AppVidaSana.Controllers.AdminWeb
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Admin - Habits")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Route("api/admin/habits")]
    [RequestTimeout("CustomPolicy")]
    public class AdminHabitsController : ControllerBase
    {
        private readonly IAWHabits _HabitService;

        public AdminHabitsController(IAWHabits HabitService)
        {
            _HabitService = HabitService;
        }

        /// <summary>
        /// This driver obtains water drinking habits records by patient.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The userFeedDate property must have the following structure:   
        ///     {
        ///        "userFeedDate": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        /// 
        ///     The userFeedTime property must have the following structure:
        ///     {
        ///         "userFeedTime": "HH:MM" (HOURS:MINUTES) 24 HOURS FORMAT
        ///     }
        ///     
        /// </remarks>
        /// <response code="200"></response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetHabitDrinkResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("drink")]
        [Produces("application/json")]
        public async Task<IActionResult> GetHabitsDrinkPerUserAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            try
            {
                var hDrink = await _HabitService.GetAllHabitsDrinkPerUserAsync(filter, page, HttpContext.RequestAborted);

                GetHabitDrinkResponse response = new GetHabitDrinkResponse
                {
                    hDrink = hDrink
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, hDrink = response.hDrink });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
        }

        /// <summary>
        /// This driver obtains drug habit records by patient.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The userFeedDate property must have the following structure:   
        ///     {
        ///        "userFeedDate": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        /// 
        ///     The userFeedTime property must have the following structure:
        ///     {
        ///         "userFeedTime": "HH:MM" (HOURS:MINUTES) 24 HOURS FORMAT
        ///     }
        ///     
        /// </remarks>
        /// <response code="200"></response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetHabitDrugsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("drugs")]
        [Produces("application/json")]
        public async Task<IActionResult> GetHabitsDrugsPerUserAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            try
            {
                var hDrugs = await _HabitService.GetAllHabitsDrugsPerUserAsync(filter, page, HttpContext.RequestAborted);

                GetHabitDrugsResponse response = new GetHabitDrugsResponse
                {
                    hDrugs = hDrugs
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, hDrugs = response.hDrugs });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
        }

        /// <summary>
        /// This driver obtains the sleep habits records per patient.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The userFeedDate property must have the following structure:   
        ///     {
        ///        "userFeedDate": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        /// 
        ///     The userFeedTime property must have the following structure:
        ///     {
        ///         "userFeedTime": "HH:MM" (HOURS:MINUTES) 24 HOURS FORMAT
        ///     }
        ///     
        /// </remarks>
        /// <response code="200"></response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetHabitSleepResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("sleep")]
        [Produces("application/json")]
        public async Task<IActionResult> GetHabitsSleepPerUserAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            try
            {
                var hSleep = await _HabitService.GetAllHabitsSleepPerUserAsync(filter, page, HttpContext.RequestAborted);

                GetHabitSleepResponse response = new GetHabitSleepResponse
                {
                    hSleep = hSleep
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, hSleep = response.hSleep });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
        }

        /// <summary>
        /// This driver obtains the monthly habit tracking records per patient.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The userFeedDate property must have the following structure:   
        ///     {
        ///        "userFeedDate": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        /// 
        ///     The userFeedTime property must have the following structure:
        ///     {
        ///         "userFeedTime": "HH:MM" (HOURS:MINUTES) 24 HOURS FORMAT
        ///     }
        ///     
        /// </remarks>
        /// <response code="200"></response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMFUsHabitsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("mfu-habit")]
        [Produces("application/json")]
        public async Task<IActionResult> GetMFUsHabitsAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            try
            {
                var mfu = await _HabitService.GetMFUsHabitsAsync(filter, page, HttpContext.RequestAborted);

                GetMFUsHabitsResponse response = new GetMFUsHabitsResponse
                {
                    mfu = mfu
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, mfu = response.mfu });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
        }
    }
}
