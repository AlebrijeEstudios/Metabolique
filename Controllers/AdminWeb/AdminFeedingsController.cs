using AppVidaSana.Api;
using AppVidaSana.ProducesResponseType;
using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.ProducesResponseType.AdminWeb;
using AppVidaSana.Services.IServices.IAdminWeb;

namespace AppVidaSana.Controllers.AdminWeb
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
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
        /// <response code="401">Returns a message indicating that the token has expired.</response> 
        /// <response code="503">Returns a message indicating that the response timeout has passed.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(GetFeedingsResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ProducesResponseType(StatusCodes.Status503ServiceUnavailable, Type = typeof(RequestTimeoutExceptionMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetFeedingsAsync([FromQuery] Guid accountID, [FromQuery] int page)
        {
            var feedings = await _FeedingService.GetAllFeedsOfAUserAsync(accountID, page, HttpContext.RequestAborted);

            GetFeedingsResponse response = new GetFeedingsResponse
            {
                feedings = feedings
            };

            return StatusCode(StatusCodes.Status200OK, new { message = response.message, feedings = response.feedings });
        }
    }
}
