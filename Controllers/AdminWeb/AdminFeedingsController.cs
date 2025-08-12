using AppVidaSana.Api;
using AppVidaSana.ProducesResponseType;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Services.IServices.IAdminWeb;
using AppVidaSana.Exceptions;
using AppVidaSana.ProducesResponseType.AdminWeb.Feeding;
using Microsoft.AspNetCore.RateLimiting;
using AppVidaSana.Models.Dtos.AdminWeb_Dtos;

namespace AppVidaSana.Controllers.AdminWeb
{
    [Authorize(Roles = "Admin,User")]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Tags("Admin - Feedings")]
    [ApiExplorerSettings(GroupName = "admin")]
    [Route("api/admin/feedings")]
    [RequestTimeout("CustomPolicy")]
    public class AdminFeedingsController : ControllerBase
    {
        private readonly IAWFeeding _FeedingService;

        public AdminFeedingsController(IAWFeeding FeedingService)
        {
            _FeedingService = FeedingService;
        }

        /// <summary>
        /// This controller returns all information about a user's power supply.
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
        /// <response code="200">Returns information from the user's feed.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFeedingsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetFeedsOfAUserAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            try 
            {
                var feedings = await _FeedingService.GetAllFeedsOfAUserAsync(filter, page, HttpContext.RequestAborted);

                GetFeedingsResponse response = new GetFeedingsResponse
                {
                    feedings = feedings
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, feedings = response.feedings });
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
        /// This controller returns all food consumed by the user.
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
        /// <response code="200">Returns information from the food consumed by the user.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFoodsResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("foods")]
        [Produces("application/json")]
        public async Task<IActionResult> GetFoodsConsumedPerUserFeedAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            try 
            { 
                var foods = await _FeedingService.GetAllFoodsConsumedPerUserFeedAsync(filter, page, HttpContext.RequestAborted);

                GetFoodsResponse response = new GetFoodsResponse
                {
                    foods = foods
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, foods = response.foods });
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
        /// The required calories per patient are obtained.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetUserCaloriesResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("calories-needed-per-user")]
        [Produces("application/json")]
        public async Task<IActionResult> GetUserCaloriesAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            try
            {
                var userCal = await _FeedingService.GetAllUserCaloriesAsync(filter, page, HttpContext.RequestAborted);

                GetUserCaloriesResponse response = new GetUserCaloriesResponse
                {
                    userCal = userCal
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, calNeeded = response.userCal });
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
        /// The monthly dietary follow-ups of the patients are obtained.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetMFUsFeedingResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("mfu-feeding")]
        [Produces("application/json")]
        public async Task<IActionResult> GetMFUsFeedingAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            try
            {
                var mfu = await _FeedingService.GetMFUsFeedingAsync(filter, page, HttpContext.RequestAborted);

                GetMFUsFeedingResponse response = new GetMFUsFeedingResponse
                {
                    mfu = mfu
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, mfu = response.mfu});
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
        /// This driver gets calories consumed per day per patient.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCalConsumedResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("calories-consumed-per-day")]
        [Produces("application/json")]
        public async Task<IActionResult> GetCaloriesConsumedPerUserAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            try
            {
                var calConsumed = await _FeedingService.GetAllCaloriesConsumedPerUserAsync(filter, page, HttpContext.RequestAborted);

                GetCalConsumedResponse response = new GetCalConsumedResponse
                {
                    calConsumed = calConsumed
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, calConsumed = response.calConsumed });
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
        /// This driver gets calories required per days per patient.
        /// </summary>
        /// <response code="200">Returns a message that the update has been successful</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="429">Returns a series of messages indicating too many requests.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetCalRequiredResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests, Type = typeof(RequestGeneralExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestGeneralExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [EnableRateLimiting("read-only")]
        [HttpGet("calories-required-per-days")]
        [Produces("application/json")]
        public async Task<IActionResult> GetCaloriesRequiredPerDaysAsync([FromQuery] FilterAdminDto filter, [FromQuery] int page)
        {
            try
            {
                var calRequired = await _FeedingService.GetAllCaloriesRequiredPerDaysAsync(filter, page, HttpContext.RequestAborted);

                GetCalRequiredResponse response = new GetCalRequiredResponse
                {
                    calRequired = calRequired
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, calRequired = response.calRequired });
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
