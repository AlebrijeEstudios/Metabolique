using AppVidaSana.Api;
using AppVidaSana.Exceptions;
using AppVidaSana.Exceptions.Feeding;
using AppVidaSana.Models.Dtos.Feeding_Dtos;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.ProducesResponseType.Feeding;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;

namespace AppVidaSana.Controllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/feeding")]
    [RequestSizeLimit(8388608)]
    [RequestFormLimits(MultipartBodyLengthLimit = 8388608)]
    [RequestTimeout("CustomPolicy")]
    public class FeedingController : ControllerBase
    {
        private readonly IFeeding _FeedingService;

        public FeedingController(IFeeding FeedingService)
        {
            _FeedingService = FeedingService;
        }

        /// <summary>
        /// This driver returns general information about a user's feed.
        /// </summary>
        /// <remarks>
        /// Sample Request:
        /// 
        ///     The dateExercise property must have the following structure:   
        ///     {
        ///        "dateExercise": "0000-00-00" (YEAR-MOUNTH-DAY)
        ///     }
        ///     
        /// </remarks>
        /// <response code="200">It returns three arrays, one with the user's default meals of the day, another with other meals of the day and the last one with the calories consumed during the last 7 days.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetInfoGeneralFeedingResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetInfoGeneralFeedingAsync([FromQuery] Guid accountID, [FromQuery] DateOnly date)
        {
            var infoFeeding = await _FeedingService.GetInfoGeneralFeedingAsync(accountID, date, HttpContext.RequestAborted);

            GetInfoGeneralFeedingResponse response = new GetInfoGeneralFeedingResponse
            {
                defaultDailyMeals = infoFeeding.defaultDailyMeals,
                othersDailyMeals = infoFeeding.othersDailyMeals,
                caloriesConsumed = infoFeeding.caloriesConsumed,
                mfuStatus = infoFeeding.mfuStatus
            };

            return StatusCode(StatusCodes.Status200OK, new
            {
                message = response.message,
                defaultDailyMeals = response.defaultDailyMeals,
                othersDailyMeals = response.othersDailyMeals,
                caloriesConsumed = response.caloriesConsumed,
                mfuStatus = response.mfuStatus
            });
        }

        /// <summary>
        /// This controller returns information about the user's feed for a given day.
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
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAddUpdateFeedingResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet("{userFeedID:guid}")]
        [Produces("application/json")]
        public async Task<IActionResult> GetFeedingAsync(Guid userFeedID)
        {
            var feeding = await _FeedingService.GetFeedingAsync(userFeedID, HttpContext.RequestAborted);

            GetAddUpdateFeedingResponse response = new GetAddUpdateFeedingResponse
            {
                feeding = feeding
            };

            return StatusCode(StatusCodes.Status200OK, new { message = response.message, feeding = response.feeding });
        }

        /// <summary>
        /// This controller captures the feeding that the user performs during the day.
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
        /// <response code="201">Returns a message that the information has been successfully stored.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(GetAddUpdateFeedingResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpPost]
        [Produces("application/json")]
        public async Task<IActionResult> AddFeedingAsync([FromBody] AddFeedingDto values)
        {
            try
            {
                var feeding = await _FeedingService.AddFeedingAsync(values, HttpContext.RequestAborted);

                GetAddUpdateFeedingResponse response = new GetAddUpdateFeedingResponse
                {
                    feeding = feeding
                };

                return StatusCode(StatusCodes.Status201Created, new { message = response.message, feeding = response.feeding });
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
        /// This controller updates the user's feed.
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
        /// <response code="200">Returns a message that the update has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="404">Returns a message indicating that no records have been found for a user feed.</response>     
        /// <response code="409">Returns a series of messages indicating that some values are invalid.</response>
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetAddUpdateFeedingResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status404NotFound, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ExceptionListMessages))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpPut]
        [Produces("application/json")]
        public async Task<IActionResult> UpdateFeedingAsync([FromBody] UpdateFeedingDto values)
        {
            try
            {
                var feeding = await _FeedingService.UpdateFeedingAsync(values, HttpContext.RequestAborted);

                GetAddUpdateFeedingResponse response = new GetAddUpdateFeedingResponse
                {
                    feeding = feeding
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, feeding = response.feeding });
            }
            catch (UnstoredValuesException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status400BadRequest, new { message = response.message, status = response.status });
            }
            catch (UserFeedNotFoundException ex)
            {
                ExceptionMessage response = new ExceptionMessage
                {
                    status = ex.Message
                };

                return StatusCode(StatusCodes.Status404NotFound, new { message = response.message, status = response.status });
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
        /// This controller deletes the record of a meal of the day.
        /// </summary>
        /// <response code="200">Returns a message that the elimination has been successful.</response>
        /// <response code="400">Returns a message that the requested action could not be performed.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(DeleteFeedingResponse))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ExceptionMessage))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpDelete("{userFeedID:guid}")]
        [Produces("application/json")]
        public async Task<IActionResult> DeleteFeedingAsync(Guid userFeedID)
        {
            try
            {
                bool status = await _FeedingService.DeleteFeedingAsync(userFeedID, HttpContext.RequestAborted);

                DeleteFeedingResponse response = new DeleteFeedingResponse
                {
                    status = status
                };

                return StatusCode(StatusCodes.Status200OK, new { message = response.message, status = response.status });
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
