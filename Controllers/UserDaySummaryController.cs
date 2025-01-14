using AppVidaSana.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.AspNetCore.Mvc;
using AppVidaSana.Api;
using AppVidaSana.ProducesResponseType;

namespace AppVidaSana.Controllers
{
    [Authorize]
    [EnableCors("RulesCORS")]
    [ApiController]
    [Route("api/summary")]
    [RequestTimeout("CustomPolicy")]
    public class UserDaySummaryController : ControllerBase
    {
        private readonly IUserDaySummary _UserDaySummaryService;

        public UserDaySummaryController(IUserDaySummary UserDaySummaryService)
        {
            _UserDaySummaryService = UserDaySummaryService;
        }

        /// <summary>
        /// This controller obtains the summary of the user's day.
        /// </summary>
        /// <response code="200">Returns account information if found.</response>
        /// <response code="401">Returns a message indicating that the token has expired.</response>
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(UserDaySummaryResponse))]
        [ProducesResponseType(StatusCodes.Status401Unauthorized, Type = typeof(ExceptionExpiredTokenMessage))]
        [ApiKeyAuthorizationFilter]
        [HttpGet]
        [Produces("application/json")]
        public async Task<IActionResult> GetUserDaySummary([FromQuery] Guid accountID, [FromQuery] DateOnly date)
        {
            var summary = await _UserDaySummaryService.GetUserDaySummaryAsync(accountID, date, HttpContext.RequestAborted);

            UserDaySummaryResponse response = new UserDaySummaryResponse
            {
                summary = summary
            };

            return StatusCode(StatusCodes.Status200OK, new { message = response.message, summary = response.summary });
        }
    }
}
